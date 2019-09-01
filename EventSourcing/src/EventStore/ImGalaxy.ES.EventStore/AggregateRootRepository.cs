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
        public AggregateRootRepository(IChangeTracker changeTracker,
            IEventDeserializer eventDeserializer,
            IEventStoreConnection eventStoreConnection,
            IEventStoreConfigurations eventStoreConfigurator,
            IStreamNameProvider streamNameProvider)
            : base(changeTracker, eventDeserializer, eventStoreConnection, eventStoreConfigurator, streamNameProvider)
        {
        }
         
        public void Add(TAggregateRoot root, string identifier) =>
            root.With(r => ChangeTracker.Attach(new Aggregate(identifier, (int) ExpectedVersion.NoStream, r)));

        public async Task AddAsync(TAggregateRoot root, string identifier) =>
            root.With(r => ChangeTracker.Attach(new Aggregate(identifier, (int) ExpectedVersion.NoStream, r)));

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromChangeTrackerIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); }

            var streamName = GetStreamNameOfRoot(identifier);

            var version = StreamPosition.Start;

            StreamEventsSlice slice = await ReadStreamEventsForwardAsync(streamName, version);
            
            slice.ThrowsIf(s=> s.Status == SliceReadStatus.StreamDeleted || s.Status == SliceReadStatus.StreamNotFound,
                new AggregateNotFoundException(streamName));

            TAggregateRoot root = IntanceOfRoot().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice)); 

            while (!slice.IsEndOfStream)
            {
                slice = await ReadStreamEventsForwardAsync(streamName, slice.NextEventNumber); 
                
                ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));
            }

            ClearChangesOfRoot(root);

            AttachAggregateToChangeTracker(identifier, (int)slice.LastEventNumber, root);

            return new Optional<TAggregateRoot>(root); 
        }

    }
}
