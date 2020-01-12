using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class CreateNewStreamHandler : IOperationHandler<CreateNewStream>
    {
        private readonly ICosmosDBClient _cosmosClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        internal CreateNewStreamHandler(ICosmosDBClient cosmosClient,
            ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _cosmosClient = cosmosClient;
            _cosmosDBConfigurations = cosmosDBConfigurations;
        } 

        async Task<IExecutionResult> IOperationHandler<CreateNewStream, IExecutionResult>.Handle(CreateNewStream operation, CancellationToken cancellationToken)
        {
            var version = operation.Events.Length;

            var newStream = CosmosStream.Create(operation.Id, operation.StreamType)
                                          .ChangeVersion(version);

            await _cosmosClient.CreateItemAsync(newStream.ToCosmosStreamDocument(),
                            this._cosmosDBConfigurations.StreamCollectionName);

            return ExecutionResult.Success;
        }
    }
}
