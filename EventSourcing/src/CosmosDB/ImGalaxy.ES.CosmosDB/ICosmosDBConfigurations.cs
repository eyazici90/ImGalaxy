﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBConfigurations
    { 
        string EndpointUri { get; set; }
        string PrimaryKey { get; set; }
        string DatabaseId { get; set; }
        int MaxLiveQueueSize { get; set; }
        int ReadBatchSize { get; set; } 
        bool IsSnapshottingOn { get; set; }
        string StreamCollectionName { get; set; }
        string EventCollectionName { get; set; }
    }
}
