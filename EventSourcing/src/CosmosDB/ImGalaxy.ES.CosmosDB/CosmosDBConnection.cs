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
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        public CosmosDBConnection(IDocumentClient cosmosClient, ICosmosDBConfigurations cosmosDBConfigurations)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
        }

        public async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count) =>
            await ReadStreamWithEventsByDirection(streamId, start, count, 
                ()=> GetEventDocumentsForwardAsync(eDoc => eDoc.StreamId == streamId, Convert.ToInt32(start), count));
     
        public async Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count) =>
              await ReadStreamWithEventsByDirection(streamId, start, count,
                () => GetEventDocumentsBackwardAsync(eDoc => eDoc.StreamId == streamId, Convert.ToInt32(start), count));
      
        public async Task AppendToStreamAsync(string streamId, long expectedVersion,
            params CosmosEventData[] events)
        {
            var id = CosmosStreamNameExtensions.GetStreamIdentifier(streamId);
            var streamType = CosmosStreamNameExtensions.GetStreamType(streamId);
            long eventPosition = 0;
            if (expectedVersion == ExpectedVersion.NoStream || expectedVersion == ExpectedVersion.Any)
            {
                var newStream = CosmosStream.Create(id, streamType);
                
                await CreateItemAsync(newStream.ToCosmosStreamDocument(), this._cosmosDBConfigurations.DatabaseId, 
                                this._cosmosDBConfigurations.StreamCollectionName);
            }
            else
            {
                var existingStream = await this.GetStreamDocumentByIdAsync(streamId);
                existingStream.ThrowsIf(stream => !existingStream.HasValue, new AggregateNotFoundException(streamId));
                              //.ThrowsIf(stream => expectedVersion <= stream.Value.Version,
                              //                      new OptimisticConcurrencyException(expectedVersion.ToString()));
            }

            foreach (var @event in events)
            {
                var newEvent = new EventDocument(@event.EventId, id, eventPosition, @event.Data,
                    @event.EventMetadata, @event.EventType);
                 
                await CreateItemAsync(newEvent, this._cosmosDBConfigurations.DatabaseId, this._cosmosDBConfigurations.EventCollectionName);

                eventPosition++;
            }
        }
        private async Task CreateItemAsync<T>(T item, string databaseId, string collectionName)=>
            await _cosmosClient.CreateDocumentAsync(
               UriFactory.CreateDocumentCollectionUri(databaseId, collectionName), item as object);

        private async Task UpdateItemAsync<T>(string id, string databaseId, string collectionName, T item) =>
        
             await _cosmosClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(
                databaseId, collectionName, id), item);
        

        private async Task<Optional<CosmosStream>> ReadStreamWithEventsByDirection(string streamId, long start, int count, Func<IEnumerable<EventDocument>> eventFunc)
        {
            var existingStream = await this.GetStreamDocumentByIdAsync(streamId);

            if (!existingStream.HasValue)
                return Optional<CosmosStream>.Empty;

            var existingEvents = eventFunc();

            var cosmosStream = existingStream.Value.ToCosmosStream(existingEvents);

            return new Optional<CosmosStream>(cosmosStream);

        }

        private  IEnumerable<EventDocument> GetEventDocumentsForwardAsync(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
           GetEventDocumentQuery(predicate)
                .OrderBy(e=>e.Position)
                .Skip(start)
                .Take(count)
                .AsEnumerable();

        private IEnumerable<EventDocument> GetEventDocumentsBackwardAsync(Expression<Func<EventDocument, bool>> predicate, int start, int count) =>
             GetEventDocumentQuery(predicate)
                .OrderByDescending(e => e.Position)
                .Skip(start)
                .Take(count)
                .AsEnumerable();

        
        private IQueryable<EventDocument> GetEventDocumentQuery(Expression<Func<EventDocument, bool>> predicate) =>
            _cosmosClient.CreateDocumentQuery<EventDocument>(
                UriFactory.CreateDocumentCollectionUri(_cosmosDBConfigurations.DatabaseId, _cosmosDBConfigurations.EventCollectionName),
                new FeedOptions { MaxItemCount = _cosmosDBConfigurations.ReadBatchSize })
                .Where(predicate);

        private async Task<Optional<StreamDocument>> GetStreamDocumentByIdAsync(string id)
        {
            try
            {
                Document document = await _cosmosClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDBConfigurations.DatabaseId,
                    _cosmosDBConfigurations.StreamCollectionName, id));

                return new Optional<StreamDocument>((StreamDocument)(dynamic)document);
            }
            catch (DocumentClientException)
            {
                return Optional<StreamDocument>.Empty;
            }
        }

    }
}
