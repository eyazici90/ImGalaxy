using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootRepository<TAggregateRoot>   where TAggregateRoot : IAggregateRoot
    {
        TAggregateRoot Get(string identifier);
        Task<TAggregateRoot> GetAsync(string identifier);

        TAggregateRoot Add(TAggregateRoot root, string identifier = default);

        Task<TAggregateRoot> AddAsync(TAggregateRoot root, string identifier = default);
    }
}
