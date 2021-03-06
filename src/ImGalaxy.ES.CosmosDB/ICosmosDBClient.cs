﻿using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBClient
    {
        IQueryable<T> GetDocumentQuery<T>(Expression<Func<T, bool>> predicate, string collectionId);
        Task CreateItemAsync<T>(T item, string collectionName);
        Task<ItemResponse<T>> ReadItemAsync<T>(string identifer, PartitionKey partitionKey, string containerName);
        Task UpdateItemAsync<T>(string id, string collectionName,
            T item, string etag);
        Task CreateDatabaseIfNotExistsAsync();
        Task CreateContainerIfNotExistsAsync(string containerName, string partitionPath);
    }
}
