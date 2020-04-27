using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBClient : ICosmosDBClient
    {
        private readonly CosmosClient _client;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        public CosmosDBClient(CosmosClient client,
            ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
        }

        public async Task CreateItemAsync<T>(T item, string containerName) =>
            await GetContainer(containerName).CreateItemAsync(item);

        public IQueryable<T> GetDocumentQuery<T>(Expression<Func<T, bool>> predicate, string containerName) =>
              GetContainer(containerName).GetItemLinqQueryable<T>(true)
                                         .Where(predicate);

        private Container GetContainer(string containerName) =>
          _client.GetContainer(_cosmosDBConfigurations.DatabaseId, containerName);

        public async Task UpdateItemAsync<T>(string id, string containerName, T item, string etag)
        {
            ItemRequestOptions requestOptions = null;
            if (!string.IsNullOrEmpty(etag))
                requestOptions = new ItemRequestOptions { IfMatchEtag = etag };

            await GetContainer(containerName).ReplaceItemAsync(item, id, requestOptions: requestOptions);
        }


        public async Task CreateDatabaseIfNotExistsAsync() =>
            await _client.CreateDatabaseIfNotExistsAsync(_cosmosDBConfigurations.DatabaseId);


        public async Task CreateContainerIfNotExistsAsync(string containerName, string partitionPath)
        {
            var database = _client.GetDatabase(_cosmosDBConfigurations.DatabaseId);

            ContainerProperties containerProperties = new ContainerProperties { Id = containerName };

            await database.CreateContainerIfNotExistsAsync(containerProperties);
        }
    }
}
