﻿using ImGalaxy.ES.CosmosDB.Documents;
using ImGalaxy.ES.CosmosDB.Internal.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class GetEventDocumentsForwardHandler : IOperationHandler<GetEventDocumentsForward, IEnumerable<EventDocument>>
    {
        private readonly ICosmosDBClient _cosmosClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        public GetEventDocumentsForwardHandler(ICosmosDBClient cosmosClient,
            ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _cosmosClient = cosmosClient;
            _cosmosDBConfigurations = cosmosDBConfigurations;
        }

        async Task<IEnumerable<EventDocument>> IOperationHandler<GetEventDocumentsForward, IEnumerable<EventDocument>>.Handle(GetEventDocumentsForward operation, CancellationToken cancellationToken)
        {
            var skipCount = operation.Start < 1 ? 0 : operation.Start - 1;
            var result = new List<EventDocument>();

            await _cosmosClient.GetDocumentQuery(operation.Predicate, _cosmosDBConfigurations.EventContainerName)
                 .OrderBy(e => e.Position)
                 .Skip(skipCount)
                 .Take(operation.Count)
                 .ToFeedIterator(eDoc => result.Add(eDoc))
                 .ConfigureAwait(false);

            return result;
        }



    }
}
