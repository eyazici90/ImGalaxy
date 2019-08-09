using ImGalaxy.ES.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class AggregateRootRepository<TAggregateRoot> : AggregateRootRepositoryBase<TAggregateRoot>, IAggregateRootRepository<TAggregateRoot>
          where TAggregateRoot : IAggregateRoot
    {
        public AggregateRootRepository(IUnitOfWork unitOfWork, IEventDeserializer eventDeserializer, 
            ICosmosDBConnection cosmosDBConnection, ICosmosDBConfigurator cosmosDBConfigurator,
            IStreamNameProvider streamNameProvider) 
            : base(unitOfWork, eventDeserializer, cosmosDBConnection, cosmosDBConfigurator, streamNameProvider)
        {
        }

        public void Add(TAggregateRoot root, string identifier = default) =>
            root.With(r => UnitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, r)));

        public async Task AddAsync(TAggregateRoot root, string identifier = default) =>
            root.With(r => UnitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, r)));

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromUnitOfWorkIfExits(identifier);

            if (!existingAggregate.HasValue) { return Optional<TAggregateRoot>.Empty; }

            var streamName = GetStreamNameOfRoot(identifier);

            var version = StreamPosition.Start;

            var slice = await ReadStreamEventsForwardAsync(streamName, version);

            if (!slice.HasValue) { return Optional<TAggregateRoot>.Empty; } 

            TAggregateRoot root = IntanceOfRoot().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice.Value));

            //while (!slice.IsEndOfStream)
            //{
            //    slice = await ReadStreamEventsForwardAsync(streamName, slice.Value.NextEventNumber);

            //    ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));
            //}

            ClearChangesOfRoot(root);

            AttachAggregateToUnitOfWork(identifier, (int)slice.Value.LastEventNumber, root);

            return new Optional<TAggregateRoot>(root);
        }

    }
}
