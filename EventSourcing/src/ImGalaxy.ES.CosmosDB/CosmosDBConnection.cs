using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBConnection : ICosmosDBConnection
    {
        private readonly ICosmosDBClient _cosmosClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        private readonly IEventSerializer _eventSerializer;
        public CosmosDBConnection(IEventSerializer eventSerializer,
            ICosmosDBClient cosmosClient,
            ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
        }

        public async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count) =>
            await ReadStreamWithEventsByDirection(streamId, start, count,
                  id => GetEventDocumentsForward(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count));


        public async Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count) =>
           await ReadStreamWithEventsByDirection(streamId, start, count,
                  id => GetEventDocumentsBackward(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count));


        public async Task<IExecutionResult> AppendToStreamAsync(string streamId, long expectedVersion,
          params CosmosEventData[] events)
        {
            var locker = AsyncStreamLockers.Get(streamId);

            await locker.LockAsync(async () =>
                await AppendToStreamInternalAsync(streamId, expectedVersion, events)
            );

            return ExecutionResult.Success;
        }
        private async Task<IExecutionResult> AppendToStreamInternalAsync(string streamId, long expectedVersion,
            params CosmosEventData[] events)
        {
            var id = CosmosStreamNameStrategy.GetStreamIdentifier(streamId);
            var streamType = CosmosStreamNameStrategy.GetStreamType(streamId);

            long eventPosition = EventPosition.Start;

            if (expectedVersion == ExpectedVersion.NoStream || expectedVersion == ExpectedVersion.Any)
            {
                await CreateNewStream(id, streamType, events);
                eventPosition++;
            }
            else
            {
                var streamDoc = await this.GetStreamDocumentByIdAsync(id);

                streamDoc.ThrowsIf(stream => !stream.HasValue, new AggregateNotFoundException(streamId));

                var existingStream = streamDoc.Value.ToCosmosStream();

                existingStream.ThrowsIf(stream => expectedVersion != stream.Version && expectedVersion != ExpectedVersion.SafeStream,
                                           new WrongExpectedStreamVersionException(expectedVersion.ToString(),
                                           existingStream.Version.ToString()));

                var streamEvents = GetEventDocumentsForward(eDoc => eDoc.StreamId == id, Convert.ToInt32(StreamPosition.Start),
                    this._cosmosDBConfigurations.ReadBatchSize);

                existingStream = existingStream.AppendEvents(streamEvents.Select(e => e.ToCosmosEvent()));

                expectedVersion = expectedVersion + events.Length;

                var newVersionedStream = existingStream.ChangeVersion(expectedVersion);

                await _cosmosClient.UpdateItemAsync(id, _cosmosDBConfigurations.StreamCollectionName, newVersionedStream.ToCosmosStreamDocument());

                eventPosition = newVersionedStream.NextEventNumber;
            }

            foreach (var @event in events)
            {
                var newEvent = new EventDocument(@event.EventId, id, eventPosition, this._eventSerializer.Serialize(@event.Data),
                    @event.EventMetadata, @event.EventType);

                await _cosmosClient.CreateItemAsync(newEvent, this._cosmosDBConfigurations.EventCollectionName);

                eventPosition++;
            }

            return ExecutionResult.Success;
        }


        private async Task<Optional<CosmosStream>> ReadStreamWithEventsByDirection(string streamId, long start, int count, Func<string, IEnumerable<EventDocument>> eventFunc)
        {

            var id = CosmosStreamNameStrategy.GetStreamIdentifier(streamId);

            var existingStream = await this.GetStreamDocumentByIdAsync(id);

            if (!existingStream.HasValue)
                return Optional<CosmosStream>.Empty;

            var existingEvents = eventFunc(id);

            var cosmosStream = existingStream.Value
                                             .ToCosmosStream(existingEvents);

            return new Optional<CosmosStream>(cosmosStream);

        }

        private IEnumerable<EventDocument> GetEventDocumentsForward(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
            _cosmosClient.GetDocumentQuery(predicate, _cosmosDBConfigurations.EventCollectionName)
                .OrderBy(e => e.Position)
                .Take(count)
                .ToList()
                .Skip(start - 1);

        private IEnumerable<EventDocument> GetEventDocumentsBackward(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
             _cosmosClient.GetDocumentQuery(predicate, _cosmosDBConfigurations.EventCollectionName)
                .OrderByDescending(e => e.Position)
                .Take(count)
                .ToList()
                .Skip(start - 1);

        private async Task<IExecutionResult> CreateNewStream(string id, string streamType, params CosmosEventData[] events)
        {
            var version = events.Length;

            var newStream = CosmosStream.Create(id, streamType)
                                          .ChangeVersion(version);

            await _cosmosClient.CreateItemAsync(newStream.ToCosmosStreamDocument(),
                            this._cosmosDBConfigurations.StreamCollectionName);

            return ExecutionResult.Success;
        }

        private async Task<Optional<StreamDocument>> GetStreamDocumentByIdAsync(string id)
        {
            try
            {
                var document = _cosmosClient.GetDocumentQuery<StreamDocument>(stream => stream.id == id, _cosmosDBConfigurations.StreamCollectionName)
                    .ToList()
                    .SingleOrDefault();

                return new Optional<StreamDocument>(document);
            }
            catch (DocumentClientException)
            {
                return Optional<StreamDocument>.Empty;
            }
        }

    }
}
