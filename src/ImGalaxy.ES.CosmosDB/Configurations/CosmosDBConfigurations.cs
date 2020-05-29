using System;
using ImGalaxy.ES.CosmosDB.Documents;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBConfigurations : ICosmosDBConfigurations
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public int MaxLiveQueueSize { get; set; }
        public int ReadBatchSize { get; set; } = 50;
        public bool IsSnapshottingOn { get; set; }
        public string StreamContainerName { get; set; } = $"Streams";
        public string EventContainerName { get; set; } = $"Events";
        public string SnapshotContainerName { get; set; }
        public int OfferThroughput { get; set; }
        public Func<EventDocument, bool> SnapshotStrategy { get; set; }
    }
}
