using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SnapshotableRootRepository<TAggregateRoot> : AggregateRootRepositoryBase<TAggregateRoot>, ISnapshotableRootRepository<TAggregateRoot>
             where TAggregateRoot : IAggregateRoot, ISnapshotable
    { 
        private readonly ISnapshotStore _snapshotStore;
        public SnapshotableRootRepository(
            ISnapshotStore snapshotStore,
            IUnitOfWork unitOfWork,
            IEventDeserializer eventDeserializer,
            IEventStoreConnection eventStoreConnection,
            IEventStoreConfigurations eventStoreConfigurator,
            IStreamNameProvider streamNameProvider)
            : base(unitOfWork, eventDeserializer, eventStoreConnection, eventStoreConfigurator, streamNameProvider) =>
            _snapshotStore = snapshotStore;

        public void Add(TAggregateRoot root, string identifier = default) => AddAsync(root, identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task AddAsync(TAggregateRoot root, string identifier = default) =>
            this.UnitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));
             
        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromUnitOfWorkIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); }

            var streamName = GetStreamNameOfRoot(identifier);

            var snapshotStreamName = StreamNameProvider.GetSnapshotStreamName(typeof(TAggregateRoot), identifier);

            Optional<Snapshot> snapshot = await _snapshotStore.GetLastSnapshot(snapshotStreamName);

            var version = StreamPosition.Start;

            snapshot.Match(s=> version = snapshot.Value.Version + 1, null);

            StreamEventsSlice slice = await ReadStreamEventsForwardAsync(streamName, version);

            slice.ThrowsIf(s => s.Status == SliceReadStatus.StreamDeleted || s.Status == SliceReadStatus.StreamNotFound,
                      new AggregateNotFoundException($"Aggregate not found by {streamName}"));
             
            TAggregateRoot root = IntanceOfRoot().Value;

            snapshot.Match(s=> root.RestoreSnapshot(snapshot.Value.State), null);

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
