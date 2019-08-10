using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootRepository<TAggregateRoot>   where TAggregateRoot : IAggregateRoot
    {
        Optional<TAggregateRoot>  Get(string identifier);
        Task<Optional<TAggregateRoot>> GetAsync(string identifier);

        void Add(TAggregateRoot root, string identifier = default);

        Task AddAsync(TAggregateRoot root, string identifier = default); 
    }
}
