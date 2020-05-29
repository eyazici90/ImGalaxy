using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using System; 

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBConfigurations : IAggregateStoreConfiguration
    { 
        string EndpointUri { get; set; }
        string PrimaryKey { get; set; }
        string DatabaseId { get; set; }
        Func<EventDocument, bool> SnapshotStrategy { get; set; }
        string StreamContainerName { get; set; }
        string EventContainerName { get; set; }
        string SnapshotContainerName { get; set; }
        int OfferThroughput { get; set; }
    }
}
