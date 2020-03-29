using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBClient
    {
        IQueryable<T> GetDocumentQuery<T>(Expression<Func<T, bool>> predicate, string collectionId); 
        Task CreateItemAsync<T>(T item, string collectionName);
        Task UpdateItemAsync<T>(string id, string collectionName,
            T item, string etag);
        Task CreateDatabaseIfNotExistsAsync();
        Task CreateCollectionIfNotExistsAsync(string collectionId);
    }
}
