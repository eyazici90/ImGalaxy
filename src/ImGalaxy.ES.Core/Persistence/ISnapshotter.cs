using Galaxy.Railway;
using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotter
    {
        bool ShouldTakeSnapshot(Type aggregateType, object @event);

        Task<IExecutionResult> TakeSnapshotAsync(string stream);
    }
}
