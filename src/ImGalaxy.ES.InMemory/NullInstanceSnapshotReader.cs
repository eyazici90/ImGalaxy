using Galaxy.Railway;
using ImGalaxy.ES.Core; 
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
