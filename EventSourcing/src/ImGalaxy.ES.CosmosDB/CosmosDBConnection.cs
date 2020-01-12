using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using ImGalaxy.ES.CosmosDB.Internal;
using ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
        private readonly IEventSerializer _eventSerializer;
        private readonly IOperationDispatcher _operationDispatcher;
        public CosmosDBConnection(IEventSerializer eventSerializer,
            ICosmosDBClient cosmosClient,
            ICosmosDBConfigurations cosmosDBConfigurations,
            IOperationDispatcher operationDispatcher)
        {
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
            _operationDispatcher = operationDispatcher ?? throw new ArgumentNullException(nameof(operationDispatcher));

            _operationDispatcher.RegisterHandler<CreateNewStream>(() => new CreateNewStreamHandler(cosmosClient, cosmosDBConfigurations));
            _operationDispatcher.RegisterHandler<GetStreamDocumentByIdAsync>(() => new GetStreamDocumentByIdAsyncHandler(cosmosClient, cosmosDBConfigurations));
            _operationDispatcher.RegisterHandler<ReadStreamWithEventsByDirection>(() => new ReadStreamWithEventsByDirectionHandler(operationDispatcher));
            _operationDispatcher.RegisterHandler<GetEventDocumentsForward>(() => new GetEventDocumentsForwardHandler(cosmosClient, cosmosDBConfigurations));

            _operationDispatcher.RegisterHandler<AppendToStreamAsync>(() =>
                new AppendToStreamAsyncHandler(eventSerializer, cosmosClient, cosmosDBConfigurations, operationDispatcher));

            _operationDispatcher.RegisterPipeline<AppendToStreamAsync>(() => new AppendToStreamAsyncPipeline());
        }

        public async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count) =>
            await ReadStreamWithEventsByDirection(streamId, start, count,
                  id => GetEventDocumentsForward(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count));


        public async Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count) =>
           await ReadStreamWithEventsByDirection(streamId, start, count,
                  id => GetEventDocumentsBackward(eDoc => eDoc.StreamId == id, Convert.ToInt32(start), count));


        public async Task<IExecutionResult> AppendToStreamAsync(string streamId, Version expectedVersion,
          params CosmosEventData[] events) =>
           await _operationDispatcher.Dispatch(new AppendToStreamAsync(streamId, expectedVersion, events));


        private async Task<Optional<CosmosStream>> ReadStreamWithEventsByDirection(string streamId, long start, int count, Func<string, IEnumerable<EventDocument>> eventFunc) =>
            await _operationDispatcher.Dispatch<ReadStreamWithEventsByDirection, Optional<CosmosStream>>
                (new ReadStreamWithEventsByDirection(streamId, start, count, eventFunc));
         
        private async Task<Optional<StreamDocument>> GetStreamDocumentByIdAsync(string id) =>
            await _operationDispatcher.Dispatch<GetStreamDocumentByIdAsync, Optional<StreamDocument>>
                (new GetStreamDocumentByIdAsync(id));

        private IEnumerable<EventDocument> GetEventDocumentsForward(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
           _operationDispatcher
            .Dispatch<GetEventDocumentsForward, IEnumerable<EventDocument>>(new GetEventDocumentsForward(predicate, start, count))
            .ConfigureAwait(false)
            .GetAwaiter().GetResult();

        private IEnumerable<EventDocument> GetEventDocumentsBackward(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
             _cosmosClient.GetDocumentQuery(predicate, _cosmosDBConfigurations.EventCollectionName)
                .OrderByDescending(e => e.Position)
                .Take(count)
                .ToList()
                .Skip(start - 1);


    }
}
