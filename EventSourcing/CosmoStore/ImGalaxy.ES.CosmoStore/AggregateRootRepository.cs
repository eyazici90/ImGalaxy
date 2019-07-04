using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmoStore
{
    public class AggregateRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot>
          where TAggregateRoot : IAggregateRoot
    {
        public TAggregateRoot Add(TAggregateRoot root, string identifier = null)
        {
            throw new NotImplementedException();
        }

        public Task<TAggregateRoot> AddAsync(TAggregateRoot root, string identifier = null)
        {
            throw new NotImplementedException();
        }

        public TAggregateRoot Get(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task<TAggregateRoot> GetAsync(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
