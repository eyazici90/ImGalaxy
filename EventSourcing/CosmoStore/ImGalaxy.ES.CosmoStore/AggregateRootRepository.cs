using ImGalaxy.ES.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStreamNameProvider _streamNameProvider;
        public AggregateRootRepository(IDocumentClient cosmoClient,
            IUnitOfWork unitOfWork,
            IStreamNameProvider streamNameProvider)
        {
            _cosmoClient = cosmoClient ?? throw new ArgumentNullException(nameof(cosmoClient));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
        }
        public void Add(TAggregateRoot root, string identifier = null) => AddAsync(root, identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task AddAsync(TAggregateRoot root, string identifier = null) =>
            this._unitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root)); 

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Aggregate existingAggregate;

            _unitOfWork.TryGet(identifier, out existingAggregate);

            if (existingAggregate != null) { return new Optional<TAggregateRoot>((TAggregateRoot)((existingAggregate).Root)); } 
    
            var streamName = _streamNameProvider.GetStreamName(typeof(TAggregateRoot), identifier);

            var document = await _cosmoClient.ReadDocumentAsync(streamName);

            var root = (TAggregateRoot)(dynamic)document.Resource;

            var aggregate = new Aggregate(identifier, (int)0, root);

            this._unitOfWork.Attach(aggregate);

            return new Optional<TAggregateRoot>(root); 
            
        }
 
    }
}
