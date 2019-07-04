using ImGalaxy.ES.Core;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmoStore
{
    public class AggregateRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot>
          where TAggregateRoot : IAggregateRoot
    {
        private readonly IDocumentClient _cosmoClient;
        public AggregateRootRepository(IDocumentClient cosmoClient)
        {
            _cosmoClient = cosmoClient ?? throw new ArgumentNullException(nameof(cosmoClient));
        }
        public TAggregateRoot Add(TAggregateRoot root, string identifier = null)
        {
            throw new NotImplementedException();
        }

        public Task<TAggregateRoot> AddAsync(TAggregateRoot root, string identifier = null)
        {
            throw new NotImplementedException();
        }

        public TAggregateRoot Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<TAggregateRoot> GetAsync(string identifier)
        {
            throw new NotImplementedException();
        }

        public async Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
