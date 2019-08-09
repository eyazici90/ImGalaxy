using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;

namespace ImGalaxy.ES.EventStore
{
    public class EventStoreConfigurations : IEventStoreConfigurations
    {
        public IEventStoreConnection Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Username { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Uri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MaxLiveQueueSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ReadBatchSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsSnapshottingOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
