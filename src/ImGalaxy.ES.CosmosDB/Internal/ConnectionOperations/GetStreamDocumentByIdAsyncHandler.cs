using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Cosmos;
using System;
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
                var result = await _cosmosClient.ReadItemAsync<StreamDocument>(operation.Id, PartitionKey.None, _cosmosDBConfigurations.StreamContainerName).ConfigureAwait(false);

                return new Optional<StreamDocument>(result.Resource);
            }
            catch (CosmosException)
            {
                return Optional<StreamDocument>.Empty;
            }
        }
    }
}
