﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBClient : ICosmosDBClient
    {
        private readonly IDocumentClient _client;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        public CosmosDBClient(IDocumentClient client, ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
        }
             
        public async Task CreateItemAsync<T>(T item, string databaseId, string collectionName) =>
         await _client.CreateDocumentAsync(
            UriFactory.CreateDocumentCollectionUri(databaseId, collectionName), item as object);

        public IQueryable<T> GetDocumentQuery<T>(Expression<Func<T, bool>> predicate, string collectionId) =>
          _client.CreateDocumentQuery<T>(
              UriFactory.CreateDocumentCollectionUri(_cosmosDBConfigurations.DatabaseId, collectionId),
              new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = _cosmosDBConfigurations.ReadBatchSize })
                 .Where(predicate);

        public async Task UpdateItemAsync<T>(string id, string collectionName, T item) => 
              await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDBConfigurations.DatabaseId,
                  collectionName, id), item);
    }
}
