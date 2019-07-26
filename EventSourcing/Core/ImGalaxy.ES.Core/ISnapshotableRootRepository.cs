using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotableRootRepository<TAggregateRoot, TSnapshot> : IAggregateRootRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, ISnapshotable<TSnapshot>
        where TSnapshot : class
    { 

    }
}
