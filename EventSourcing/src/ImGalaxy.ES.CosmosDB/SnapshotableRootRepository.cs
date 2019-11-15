using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class SnapshotableRootRepository<TAggregateRoot> : AggregateRootRepositoryBase<TAggregateRoot>, IAggregateRootRepository<TAggregateRoot>
             where TAggregateRoot : IAggregateRoot, ISnapshotable
    {
        private readonly ISnapshotReader _snapshotReader;
        public SnapshotableRootRepository(ISnapshotReader snapshotReader,
           IAggregateRootRepositoryBaseDependencies dependencies)
            : base(dependencies) =>
            _snapshotReader = snapshotReader;

        public void Add(TAggregateRoot root, string identifier) => AddAsync(root, identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task AddAsync(TAggregateRoot root, string identifier) =>
            this.ChangeTracker.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

       
        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromChangeTrackerIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); }

            var streamName = GetStreamNameOfRoot(identifier);

            var snapshotStreamName = StreamNameProvider.GetSnapshotStreamName(typeof(TAggregateRoot), identifier);

            Optional<Snapshot> snapshot = await _snapshotReader.GetLastSnapshot(snapshotStreamName);

            var version = StreamPosition.Start;

            if (snapshot.HasValue)
                version = snapshot.Value.Version + 1;
            
            var slice = await ReadStreamEventsForwardAsync(streamName, version);

            if (!slice.HasValue) { return Optional<TAggregateRoot>.Empty; }

            TAggregateRoot root = IntanceOfRoot().Value;

            if (snapshot.HasValue)
                root.RestoreSnapshot(snapshot.Value.State);
                     
            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice.Value));
             
            ClearChangesOfRoot(root);

            AttachAggregateToChangeTracker(identifier, (int)slice.Value.Version, root);
            
            return new Optional<TAggregateRoot>(root);
        }
    }
}
