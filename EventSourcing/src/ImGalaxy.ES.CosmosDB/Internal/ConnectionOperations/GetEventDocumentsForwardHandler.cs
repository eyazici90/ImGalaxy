using ImGalaxy.ES.CosmosDB.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        async Task<IEnumerable<EventDocument>> IOperationHandler<GetEventDocumentsForward, IEnumerable<EventDocument>>.Handle(GetEventDocumentsForward operation, CancellationToken cancellationToken)=>
        
            _cosmosClient.GetDocumentQuery(operation.Predicate, _cosmosDBConfigurations.EventCollectionName)
                 .OrderBy(e => e.Position)
                 .Take(operation.Count )
                 .ToList()
                 .Skip(operation.Start - 1);
        
    }
}
