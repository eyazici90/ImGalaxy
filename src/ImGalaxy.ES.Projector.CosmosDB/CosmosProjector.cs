using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector.CosmosDB
{
    public class CosmosProjector : Projector
    {
        public CosmosProjector(IDocumentClient documentClient,
            CosmosOptions cosmosOptions)
            : base(async id => await Get(documentClient, cosmosOptions, id),
                   async state => await Upsert(documentClient, cosmosOptions, state),
                   type => FindProjection(type))
        {
        }

        private static async Task<dynamic> Get(IDocumentClient documentClient,
            CosmosOptions cosmosOptions,
            string id)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(cosmosOptions.Database,
                cosmosOptions.Collection);

            var doc = documentClient.CreateDocumentQuery<Document>(collectionUri)
                        .Where(r => r.Id == id)
                        .AsEnumerable()
                        .SingleOrDefault();

            return (dynamic)doc;
        }

        private static async Task Upsert(IDocumentClient documentClient,
            CosmosOptions cosmosOptions,
            object state)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(cosmosOptions.Database, cosmosOptions.Collection);

            await documentClient.UpsertDocumentAsync(collectionUri, state);
        }

        private static object FindProjection(Type type)
        {
            var projectionType = ProjectionDefiner.GetProjectionType(type);

            return Activator.CreateInstance(projectionType, true);
        }
    }
}
