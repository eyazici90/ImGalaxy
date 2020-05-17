using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Cosmos;
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class GetStreamDocumentByIdAsyncHandler : IOperationHandler<GetStreamDocumentByIdAsync, Optional<StreamDocument>>
    {
        private readonly ICosmosDBClient _cosmosClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        internal GetStreamDocumentByIdAsyncHandler(ICosmosDBClient cosmosClient,
            ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _cosmosClient = cosmosClient;
            _cosmosDBConfigurations = cosmosDBConfigurations;
        }

        async Task<Optional<StreamDocument>> IOperationHandler<GetStreamDocumentByIdAsync, Optional<StreamDocument>>.Handle(GetStreamDocumentByIdAsync operation, CancellationToken cancellationToken)
        {
            try
            {
                var document = _cosmosClient.GetDocumentQuery<StreamDocument>(stream => stream.id == operation.Id, _cosmosDBConfigurations.StreamContainerName)
                    .ToList()
                    .SingleOrDefault();

                return new Optional<StreamDocument>(document);
            }
            catch (CosmosException)
            {
                return Optional<StreamDocument>.Empty;
            }
        }
    }
}
