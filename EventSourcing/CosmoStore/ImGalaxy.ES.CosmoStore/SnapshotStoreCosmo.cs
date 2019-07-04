using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmoStore
{
    public class SnapshotStoreCosmo<TAggregateRoot, TSnapshot> : ISnapshotStore where TAggregateRoot : IAggregateRoot, ISnapshotable
    {
        public Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream)
        {
            throw new NotImplementedException();
        }

        public bool ShouldTakeSnapshot(Type aggregateType, object @event)
        {
            throw new NotImplementedException();
        }

        public Task TakeSnapshot(string stream)
        {
            throw new NotImplementedException();
        }
    }
}
