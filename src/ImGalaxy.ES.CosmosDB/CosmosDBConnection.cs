using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using ImGalaxy.ES.CosmosDB.Internal;
using ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Version = ImGalaxy.ES.Core.Version;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBConnection : ICosmosDBConnection
    {
        private readonly ICosmosDBClient _cosmosClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        private readonly IOperationDispatcher _operationDispatcher;
        public CosmosDBConnection(IEventSerializer eventSerializer,
            ICosmosDBClient cosmosClient,
            ICosmosDBConfigurations cosmosDBConfigurations,
            IOperationDispatcher operationDispatcher)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
            _operationDispatcher = operationDispatcher ?? throw new ArgumentNullException(nameof(operationDispatcher));

            _operationDispatcher.RegisterHandler<CreateNewStream>(() =>
                new CreateNewStreamHandler(async doc => await _cosmosClient.CreateItemAsync(doc, _cosmosDBConfigurations.StreamContainerName)));

            _operationDispatcher.RegisterHandler<GetStreamDocumentByIdAsync>(() => new GetStreamDocumentByIdAsyncHandler(cosmosClient, cosmosDBConfigurations));

            _operationDispatcher.RegisterHandler<ReadStreamWithEventsByDirection>(() =>
                new ReadStreamWithEventsByDirectionHandler(async opt => await _operationDispatcher
                                                                        .Dispatch<GetStreamDocumentByIdAsync, Optional<StreamDocument>>(opt)));

            _operationDispatcher.RegisterHandler<GetEventDocumentsForward>(() => new GetEventDocumentsForwardHandler(cosmosClient, cosmosDBConfigurations));

            _operationDispatcher.RegisterHandler<AppendToStreamAsync>(() =>
                new AppendToStreamAsyncHandler(eventSerializer, cosmosClient, cosmosDBConfigurations, operationDispatcher));

            _operationDispatcher.RegisterPipeline<AppendToStreamAsync>(() => new AppendToStreamAsyncPipeline());
        }

        public async Task<IExecutionResult> AppendToStreamAsync(string streamId, Version expectedVersion, params CosmosEventData[] events) =>
            await _operationDispatcher.Dispatch(new AppendToStreamAsync(streamId, expectedVersion, events)).ConfigureAwait(false);

        public async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count) =>
            await ReadStreamWithEventsByDirection(streamId, start, count,
                  id => GetEventDocumentsForward(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count)).ConfigureAwait(false);

        public async Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count) =>
           await ReadStreamWithEventsByDirection(streamId, start, count,
                  id => GetEventDocumentsBackward(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count)).ConfigureAwait(false);

        private async Task<Optional<CosmosStream>> ReadStreamWithEventsByDirection(string streamId, long start, int count, Func<string, Task<IEnumerable<EventDocument>>> eventFunc) =>
            await _operationDispatcher.Dispatch<ReadStreamWithEventsByDirection, Optional<CosmosStream>>
                (new ReadStreamWithEventsByDirection(streamId, start, count, eventFunc)).ConfigureAwait(false);

        private async Task<IEnumerable<EventDocument>> GetEventDocumentsForward(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
         await _operationDispatcher
            .Dispatch<GetEventDocumentsForward, IEnumerable<EventDocument>>(new GetEventDocumentsForward(predicate, start, count))
            .ConfigureAwait(false);

        private async Task<IEnumerable<EventDocument>> GetEventDocumentsBackward(Expression<Func<EventDocument, bool>> predicate, int start, int count)
        {
            var skipCount = start < 1 ? 0 : start - 1;
            return await Task.FromResult(_cosmosClient.GetDocumentQuery(predicate, _cosmosDBConfigurations.EventContainerName)
                 .OrderByDescending(e => e.Position)
                 .Skip(skipCount)
                 .Take(count)
                 .ToList());
        }

    }
}
