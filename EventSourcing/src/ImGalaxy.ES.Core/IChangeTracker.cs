using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IChangeTracker
    {
        IExecutionResult Attach(Aggregate aggregate);
        bool TryGet(string identifier, out Aggregate aggregate);

        bool HasChanges();

        IEnumerable<Aggregate> GetChanges();
        IExecutionResult ResetChanges();
    }
}
