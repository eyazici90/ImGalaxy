using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public class EventStoreStreamNameProvider : IStreamNameProvider
    {
        public string GetSnapshotStreamName(object aggregateRoot, string identifier)
        {
            throw new NotImplementedException();
        }

        public string GetSnapshotStreamName(Type aggregateRootType, string identifier)
        {
            throw new NotImplementedException();
        }

        public string GetStreamName(object aggregateRoot, string identifier)
        {
            throw new NotImplementedException();
        }

        public string GetStreamName(Type aggregateRootType, string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
