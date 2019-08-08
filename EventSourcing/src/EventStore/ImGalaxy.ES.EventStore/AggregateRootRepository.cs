using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class AggregateRootRepository<TAggregateRoot> : AggregateRootRepositoryBase<TAggregateRoot>, IAggregateRootRepository<TAggregateRoot>
            where TAggregateRoot : IAggregateRoot
    {
        public AggregateRootRepository(IUnitOfWork unitOfWork,
            IEventDeserializer eventDeserializer,
            IEventStoreConnection eventStoreConnection,
            IEventStoreConfigurator eventStoreConfigurator,
            IStreamNameProvider streamNameProvider)
            : base(unitOfWork, eventDeserializer, eventStoreConnection, eventStoreConfigurator, streamNameProvider)
        {
        }
         
        public void Add(TAggregateRoot root, string identifier = default) =>
            root.With(r => UnitOfWork.Attach(new Aggregate(identifier, (int) ExpectedVersion.NoStream, r)));

        public async Task AddAsync(TAggregateRoot root, string identifier = default) =>
            root.With(r => UnitOfWork.Attach(new Aggregate(identifier, (int) ExpectedVersion.NoStream, r)));

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromUnitOfWorkIfExits(identifier);

            if (!existingAggregate.HasValue) { return Optional<TAggregateRoot>.Empty; }
             
            var streamName = GetStreamNameOfRoot(identifier);

            var version = StreamPosition.Start;

            StreamEventsSlice slice = await ReadStreamEventsForwardAsync(streamName, version);
            
            slice.ThrowsIf(s=> s.Status == SliceReadStatus.StreamDeleted || s.Status == SliceReadStatus.StreamNotFound,
                new AggregateNotFoundException($"Aggregate not found by {streamName}"));

            TAggregateRoot root = IntanceOfRoot().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice)); 

            while (!slice.IsEndOfStream)
            {
                slice = await ReadStreamEventsForwardAsync(streamName, slice.NextEventNumber); 

                ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));
            }

            ClearChangesOfRoot(root);

            AttachAggregateToUnitOfWork(identifier, (int)slice.LastEventNumber, root);

            return new Optional<TAggregateRoot>(root); 
        }

    }
}
