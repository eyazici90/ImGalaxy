using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;

namespace ImGalaxy.ES.EventStore
{
    public class EventStoreConfigurations : IEventStoreConfigurations
    {
        public IEventStoreConnection Connection { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Uri { get; set; }
        public int MaxLiveQueueSize { get; set; }
        public int ReadBatchSize { get; set; } = 50;
        public bool IsSnapshottingOn { get; set; }
    }
}
