using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public interface IStreamNameProvider
    {
        string GetStreamName(object aggregateRoot, string identifier);
        string GetSnapshotStreamName(object aggregateRoot, string identifier);
    }
}
