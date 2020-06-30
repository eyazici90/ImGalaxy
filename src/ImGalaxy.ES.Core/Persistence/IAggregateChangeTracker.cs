using Galaxy.Railway; 
using System.Collections.Generic; 

namespace ImGalaxy.ES.Core
{
    public interface IAggregateChangeTracker
    {
        IExecutionResult Attach(Aggregate aggregate);
        bool TryGet(string identifier, out Aggregate aggregate);

        bool HasChanges();

        IEnumerable<Aggregate> GetChanges();
        IExecutionResult ResetChanges();
    }
}
