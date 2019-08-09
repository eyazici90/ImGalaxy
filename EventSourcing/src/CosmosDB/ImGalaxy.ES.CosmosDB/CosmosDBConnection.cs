using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBConnection : ICosmosDBConnection
    {
        private readonly IDocumentClient _cosmosClient;
        private readonly ICosmosDBConfigurator _cosmosDBConfigurator;
        public CosmosDBConnection(IDocumentClient cosmosClient, ICosmosDBConfigurator cosmosDBConfigurator)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosDBConfigurator = cosmosDBConfigurator ?? throw new ArgumentNullException(nameof(cosmosDBConfigurator));
        }
        
        public async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count)
        {
            var existingStream = await this.GetStreamDocumentByIdAsync(streamId);

            if (!existingStream.HasValue)
                return Optional<CosmosStream>.Empty; 

            var existingEvents = await this.GetEventDocumentsAsync(eDoc=> eDoc.StreamId==streamId, Convert.ToInt32(start), count);

            var cosmosStream = existingStream.Value.ToCosmosStream(existingEvents);

            return new Optional<CosmosStream>(cosmosStream);
        }
        public async Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count) =>
           await ReadStreamEventsForwardAsync(streamId, start, count);

        public async Task AppendToStreamAsync(string streamId, long expectedVersion, params CosmosEventData[] events)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<EventDocument>> GetEventDocumentsAsync(Expression<Func<EventDocument, bool>> predicate, int start, int count)
        {
            IDocumentQuery<EventDocument> query = _cosmosClient.CreateDocumentQuery<EventDocument>(
                UriFactory.CreateDocumentCollectionUri(_cosmosDBConfigurator.DatabaseId, _cosmosDBConfigurator.EventCollectionName),
                new FeedOptions { MaxItemCount = _cosmosDBConfigurator.ReadBatchSize })
                .Where(predicate)
                .Skip(start)
                .Take(count)
                .AsDocumentQuery();

            List<EventDocument> results = new List<EventDocument>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<EventDocument>());
            } 

            return results;
        }

        private async Task<Optional<StreamDocument>> GetStreamDocumentByIdAsync(string id)
        {
            try
            {
                Document document = await _cosmosClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDBConfigurator.DatabaseId,
                    _cosmosDBConfigurator.StreamCollectionName, id));

                return new Optional<StreamDocument>((StreamDocument)(dynamic)document);
            }
            catch (DocumentClientException )
            {
                return Optional<StreamDocument>.Empty;
            }
        }

    }
}
