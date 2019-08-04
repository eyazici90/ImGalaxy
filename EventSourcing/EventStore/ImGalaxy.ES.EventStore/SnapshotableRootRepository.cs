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
            IEventStoreConfigurator eventStoreConfigurator,
            IStreamNameProvider streamNameProvider)
            : base(unitOfWork, eventDeserializer, eventStoreConnection, eventStoreConfigurator, streamNameProvider) =>
            _snapshotStore = snapshotStore;

        public TAggregateRoot Add(TAggregateRoot root, string identifier = default) => AddAsync(root, identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<TAggregateRoot> AddAsync(TAggregateRoot root, string identifier = default)
        {
            this.UnitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));
            return root;
        }

        public TAggregateRoot Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<TAggregateRoot> GetAsync(string identifier)
        {
            Aggregate existingAggregate = GetAggregateFromUnitOfWorkIfExits(identifier);

            if (existingAggregate != null) { return IntanceOfRoot(existingAggregate); }

            var streamName = GetStreamNameOfRoot(identifier);

            var snapshotStreamName = StreamNameProvider.GetSnapshotStreamName(typeof(TAggregateRoot), identifier);

            Optional<Snapshot> snapshot = await _snapshotStore.GetLastSnapshot(snapshotStreamName);

            var version = StreamPosition.Start;

            if (snapshot.HasValue) { version = snapshot.Value.Version + 1; }

            StreamEventsSlice slice = await ReadStreamEventsForwardAsync(streamName, version);

            slice.ThrowsIf(s => s.Status == SliceReadStatus.StreamDeleted || s.Status == SliceReadStatus.StreamNotFound,
                      new AggregateNotFoundException($"Aggregate not found by {streamName}"));
             
            TAggregateRoot root = IntanceOfRoot();

            if (snapshot.HasValue)
            {
                root.RestoreSnapshot(snapshot.Value.State);
            }
            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));
             
            while (!slice.IsEndOfStream)
            {
                slice = await ReadStreamEventsForwardAsync(streamName, slice.NextEventNumber);

                ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));
            }

            ClearChangesOfRoot(root);

            return AttachAggregateToUnitOfWork(identifier, (int)slice.LastEventNumber, root);
        }
    }
}
