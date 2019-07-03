using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotableRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, ISnapshotable
    { 

    }
}
