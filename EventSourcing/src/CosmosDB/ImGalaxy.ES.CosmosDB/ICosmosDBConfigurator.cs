﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBConfigurator
    { 
        string EndpointUri { get; set; }
        string PrimaryKey { get; set; }
        string DatabaseId { get; set; }
        int MaxLiveQueueSize { get; set; }
        int ReadBatchSize { get; set; } 
        bool IsSnapshottingOn { get; set; }
        string StreamCollectionName { get; }
        string EventCollectionName { get; }
    }
}
