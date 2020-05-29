using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;
using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class AppendToStreamAsyncHandler : IOperationHandler<AppendToStreamAsync>
    {
        private readonly ICosmosDBClient _cosmosClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        private readonly IEventSerializer _eventSerializer;
        private readonly IOperationDispatcher _operationDispatcher;

        public AppendToStreamAsyncHandler(IEventSerializer eventSerializer,
        ICosmosDBClient cosmosClient,
        ICosmosDBConfigurations cosmosDBConfigurations,
        IOperationDispatcher operationDispatcher)
        {
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
            _operationDispatcher = operationDispatcher ?? throw new ArgumentNullException(nameof(operationDispatcher));

        }


        async Task<IExecutionResult> IOperationHandler<AppendToStreamAsync, IExecutionResult>.Handle(AppendToStreamAsync operation, CancellationToken cancellationToken)
        {
            var id = CosmosStreamNameStrategy.GetStreamIdentifier(operation.StreamId);
            var streamType = CosmosStreamNameStrategy.GetStreamType(operation.StreamId);

            long eventPosition = EventPosition.Start;

            if (operation.ExpectedVersion == ExpectedVersion.NoStream || operation.ExpectedVersion == ExpectedVersion.Any)
            {
                await _operationDispatcher.Dispatch(new CreateNewStream(id, streamType, operation.Events)).ConfigureAwait(false);
                eventPosition++;
            }
            else
            {
                var streamDoc = await _operationDispatcher
                         .Dispatch<GetStreamDocumentByIdAsync, Optional<StreamDocument>>(new GetStreamDocumentByIdAsync(id)).ConfigureAwait(false);

                streamDoc.ThrowsIf(stream => !stream.HasValue, new AggregateNotFoundException(operation.StreamId));

                var existingStream = streamDoc.Value.ToCosmosStream();

                existingStream.ThrowsIf(stream => operation.ExpectedVersion.Value != stream.Version.Value && operation.ExpectedVersion != ExpectedVersion.SafeStream,
                                           new WrongExpectedStreamVersionException(operation.ExpectedVersion.Value.ToString(),
                                           existingStream.Version.Value.ToString()));

                var streamEvents = await _operationDispatcher
                    .Dispatch<GetEventDocumentsForward, IEnumerable<EventDocument>>
                    (
                        new GetEventDocumentsForward(eDoc => eDoc.StreamId == id, Convert.ToInt32(StreamPosition.Start),
                         _cosmosDBConfigurations.ReadBatchSize)
                    ).ConfigureAwait(false);

                existingStream = existingStream.AppendEvents(streamEvents.Select(e => e.ToCosmosEvent()));

                operation.ExpectedVersion = operation.ExpectedVersion.WithVersion(operation.ExpectedVersion + operation.Events.Length);

                var newVersionedStream = existingStream.ChangeVersion(operation.ExpectedVersion);

                await _cosmosClient.UpdateItemAsync(id, _cosmosDBConfigurations.StreamContainerName,
                    newVersionedStream.ToCosmosStreamDocument(),
                     operation.ExpectedVersion.MetaData).ConfigureAwait(false);

                eventPosition = newVersionedStream.NextEventNumber;
            }

            foreach (var @event in operation.Events)
            {
                var newEvent = new EventDocument(@event.EventId, id, eventPosition, this._eventSerializer.Serialize(@event.Data),
                    @event.EventMetadata, @event.EventType);

                await _cosmosClient.CreateItemAsync(newEvent, this._cosmosDBConfigurations.EventContainerName).ConfigureAwait(false);

                eventPosition++;
            }

            return ExecutionResult.Success;
        } 

    }
}
