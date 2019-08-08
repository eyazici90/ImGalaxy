using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotStore
    {
        Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream);

        bool ShouldTakeSnapshot(Type aggregateType, object @event);

        Task TakeSnapshot(string stream);
    }
}
