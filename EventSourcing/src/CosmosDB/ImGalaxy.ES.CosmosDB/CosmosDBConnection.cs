using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
                id => GetEventDocumentsForwardAsync(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count));
     
        public async Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count) =>
              await ReadStreamWithEventsByDirection(streamId, start, count,
                id => GetEventDocumentsBackwardAsync(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count));
      
        public async Task<IExecutionResult> AppendToStreamAsync(string streamId, long expectedVersion,
            params CosmosEventData[] events)
        {
            var id = CosmosStreamNameExtensions.GetStreamIdentifier(streamId);
            var streamType = CosmosStreamNameExtensions.GetStreamType(streamId);

            long eventPosition = EventPosition.Start;

            if (expectedVersion == ExpectedVersion.NoStream || expectedVersion == ExpectedVersion.Any)
            {
                await CreateNewStream(id, streamType, events);
                eventPosition++;
            }
            else
            {
                var existingStream = await this.ReadStreamEventsForwardAsync(streamId, StreamPosition.Start, 
                    this._cosmosDBConfigurations.ReadBatchSize);

                existingStream.ThrowsIf(stream => !existingStream.HasValue, new AggregateNotFoundException(streamId))
                              .ThrowsIf(stream => expectedVersion != stream.Value.Version,
                                                  new WrongExpectedStreamVersionException(expectedVersion.ToString(), 
                                                  existingStream.Value.Version.ToString()));

                expectedVersion = expectedVersion + events.Length;

                var newVersionedStream = existingStream.Value.ChangeVersion(expectedVersion)
                                                             .ToCosmosStreamDocument();

                await _cosmosClient.UpdateItemAsync(id, _cosmosDBConfigurations.StreamCollectionName, newVersionedStream);

                eventPosition = existingStream.Value.NextEventNumber;
            } 

            foreach (var @event in events)
            {
                var newEvent = new EventDocument(@event.EventId, id, eventPosition, this._eventSerializer.Serialize(@event.Data),
                    @event.EventMetadata, @event.EventType);
                 
                await _cosmosClient.CreateItemAsync(newEvent, this._cosmosDBConfigurations.EventCollectionName);

                eventPosition++;
            }

            return ExecutionResult.Success();
        }
    

        private async Task<Optional<CosmosStream>> ReadStreamWithEventsByDirection(string streamId, long start, int count, Func<string,IEnumerable<EventDocument>> eventFunc)
        {

            var id = CosmosStreamNameExtensions.GetStreamIdentifier(streamId); 

            var existingStream = await this.GetStreamDocumentByIdAsync(id);

            if (!existingStream.HasValue)
                return Optional<CosmosStream>.Empty;

            var existingEvents = eventFunc(id);

            var cosmosStream = existingStream.Value
                                             .ToCosmosStream(existingEvents);

            return new Optional<CosmosStream>(cosmosStream);

        }

        //https://github.com/Azure/azure-cosmos-dotnet-v3/issues/8
        private IEnumerable<EventDocument> GetEventDocumentsForwardAsync(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
            _cosmosClient.GetDocumentQuery(predicate, _cosmosDBConfigurations.EventCollectionName)
                .OrderBy(e=>e.Position) 
                .Take(count)
                .ToList()
                .Skip(start-1);
        
        //https://github.com/Azure/azure-cosmos-dotnet-v3/issues/8
        private IEnumerable<EventDocument> GetEventDocumentsBackwardAsync(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
             _cosmosClient.GetDocumentQuery(predicate, _cosmosDBConfigurations.EventCollectionName)
                .OrderByDescending(e => e.Position) 
                .Take(count)
                .ToList()
                .Skip(start-1);

        private async Task<IExecutionResult> CreateNewStream(string id, string streamType, params CosmosEventData[] events)
        {
            var version = events.Length;

            var newStream = CosmosStream.Create(id, streamType)
                                          .ChangeVersion(version);

            await _cosmosClient.CreateItemAsync(newStream.ToCosmosStreamDocument(),
                            this._cosmosDBConfigurations.StreamCollectionName);

            return ExecutionResult.Success();
        }

        private async Task<Optional<StreamDocument>> GetStreamDocumentByIdAsync(string id)
        {
            try
            {  
                var document = _cosmosClient.GetDocumentQuery<StreamDocument>(stream => stream.OriginalId == id, _cosmosDBConfigurations.StreamCollectionName)
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
