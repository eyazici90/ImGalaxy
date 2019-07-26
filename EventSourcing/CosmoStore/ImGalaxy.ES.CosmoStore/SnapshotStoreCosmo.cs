using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmoStore
{
    public class SnapshotStoreCosmo<TAggregateRoot, TSnapshot> : ISnapshotStore 
        where TAggregateRoot : IAggregateRoot, ISnapshotable<TSnapshot>
        where TSnapshot : class
    {
        public async Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream)
        {
            throw new NotImplementedException();
        }

        public bool ShouldTakeSnapshot(Type aggregateType, object @event)
        {
            throw new NotImplementedException();
        }

        public async Task TakeSnapshot(string stream)
        {
            throw new NotImplementedException();
        }
    }
}
