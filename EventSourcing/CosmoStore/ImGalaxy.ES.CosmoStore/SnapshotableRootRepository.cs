using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmoStore
{
    public class SnapshotableRootRepository<TAggregateRoot> : ISnapshotableRootRepository<TAggregateRoot>
            where TAggregateRoot : IAggregateRoot, ISnapshotable
    {
        public void Add(TAggregateRoot root, string identifier = null)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(TAggregateRoot root, string identifier = null)
        {
            throw new NotImplementedException();
        }

        public Optional<TAggregateRoot> Get(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
