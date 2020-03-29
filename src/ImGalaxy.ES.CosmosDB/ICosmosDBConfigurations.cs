using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBConfigurations : IAggregateStoreConfiguration
    { 
        string EndpointUri { get; set; }
        string PrimaryKey { get; set; }
        string DatabaseId { get; set; }
        Func<EventDocument, bool> SnapshotStrategy { get; set; }
        string StreamCollectionName { get; set; }
        string EventCollectionName { get; set; }
        string SnapshotCollectionName { get; set; }
        int OfferThroughput { get; set; }
    }
}
