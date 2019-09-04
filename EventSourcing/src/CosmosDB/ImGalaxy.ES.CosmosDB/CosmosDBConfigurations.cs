﻿using System;
using System.Collections.Generic;
using System.Text;
using ImGalaxy.ES.CosmosDB.Documents;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBConfigurations : ICosmosDBConfigurations
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public int MaxLiveQueueSize { get; set; }
        public int ReadBatchSize { get; set; }
        public bool IsSnapshottingOn { get; set; } 
        public string StreamCollectionName { get; set; } 
        public string EventCollectionName { get; set; }
        public int OfferThroughput { get; set; }
        public string SnapshotCollectionName { get; set; }
        public Func<EventDocument, bool> SnapshotStrategy { get ; set ; }
    }
}