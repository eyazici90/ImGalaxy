using Galaxy.Railway;
using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Cosmos;
using System; 
using System.Linq; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class SnapshotReaderCosmosDB : ISnapshotReader
    {  
        private readonly ICosmosDBClient _cosmosDbClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations; 
        private readonly IEventDeserializer _deserializer;
        public SnapshotReaderCosmosDB(
            ICosmosDBClient cosmosDbClient,
            ICosmosDBConfigurations cosmosDBConfigurations,
            IEventDeserializer deserializer)
        {
            _cosmosDbClient = cosmosDbClient ?? throw new ArgumentNullException(nameof(cosmosDbClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }
        public async Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream)
        {
            var existingSnapshot = await GetLastSnapshotStreamDocumentByIdAsync(snapshotStream);

            if (!existingSnapshot.HasValue)
                return Optional<Snapshot>.Empty;
             
            var eState = _deserializer.Deserialize(Type.GetType(existingSnapshot.Value.Type, true), existingSnapshot.Value.State);

            return new Optional<Snapshot>(new Snapshot(int.Parse(existingSnapshot.Value.EventPosition), eState));
        }
        private async Task<Optional<SnapshotDocument>> GetLastSnapshotStreamDocumentByIdAsync(string id)
        {
            try
            {
                var document = _cosmosDbClient
                    .GetDocumentQuery<SnapshotDocument>(stream => stream.StreamId == id, _cosmosDBConfigurations.SnapshotContainerName)
                    .OrderByDescending(s => s.EventPosition)
                    .ToList()
                    .FirstOrDefault();

                return document.ToOptional();
            }
            catch (CosmosException)
            {
                return Optional<SnapshotDocument>.Empty;
            }
        }
    }
}
