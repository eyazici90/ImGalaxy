using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateStoreConfiguration
    {
        int MaxLiveQueueSize { get; set; }
        int ReadBatchSize { get; set; }
        bool IsSnapshottingOn { get; set; }
    }
}
