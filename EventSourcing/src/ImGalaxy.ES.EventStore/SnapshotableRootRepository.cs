using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SnapshotableRootRepository<TAggregateRoot> : AggregateRootRepositoryBase<TAggregateRoot>, IAggregateRootRepository<TAggregateRoot>
             where TAggregateRoot : IAggregateRoot, ISnapshotable
    { 
        private readonly ISnapshotReader _snapshotStore;
        public SnapshotableRootRepository(
            ISnapshotReader snapshotStore,
            IAggregateRootRepositoryBaseDependencies dependencies)
            : base(dependencies) =>
            _snapshotStore = snapshotStore;

        public void Add(TAggregateRoot root, string identifier = default) => AddAsync(root, identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task AddAsync(TAggregateRoot root, string identifier = default) =>
            this.ChangeTracker.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));
             
        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromChangeTrackerIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); }

            var streamName = GetStreamNameOfRoot(identifier);

            var snapshotStreamName = StreamNameProvider.GetSnapshotStreamName(typeof(TAggregateRoot), identifier);

            Optional<Snapshot> snapshot = await _snapshotStore.GetLastSnapshot(snapshotStreamName);

            var version = StreamPosition.Start;

            if (snapshot.HasValue)
                version = snapshot.Value.Version + 1;

            StreamEventsSlice slice = await ReadStreamEventsForwardAsync(streamName, version);

            slice.ThrowsIf(s => s.Status == SliceReadStatus.StreamDeleted || s.Status == SliceReadStatus.StreamNotFound,
                      new AggregateNotFoundException(streamName));
             
            TAggregateRoot root = IntanceOfRoot().Value;

            if (snapshot.HasValue)
                root.RestoreSnapshot(snapshot.Value.State);

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
