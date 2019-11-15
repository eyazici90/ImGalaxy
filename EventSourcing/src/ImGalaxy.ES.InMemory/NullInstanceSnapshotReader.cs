using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{
    public class NullInstanceSnapshotReader : ISnapshotReader
    {
        public async Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream)
        {
            return new Optional<Snapshot>();
        }
    }
}
