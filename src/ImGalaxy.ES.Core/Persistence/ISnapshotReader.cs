using Galaxy.Railway; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotReader
    {
        Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream);
    }
}
