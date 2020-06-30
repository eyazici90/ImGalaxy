using Galaxy.Railway; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public class SnapshotableRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot>
            where TAggregateRoot : class, IAggregateRootState<TAggregateRoot>, IAggregateRoot, ISnapshotable
    { 
        private readonly ISnapshotReader _snapshotStore;
        private readonly IAggregateStore _aggregateStore;
        private readonly IAggregateChangeTracker _changeTracker;
        private readonly IStreamNameProvider _streamNameProvider;
        public SnapshotableRootRepository(ISnapshotReader snapshotStore,
            IAggregateStore aggregateStore,
            IAggregateChangeTracker changeTracker,
            IStreamNameProvider streamNameProvider)
        {
            _snapshotStore = snapshotStore;
            _aggregateStore = aggregateStore;
            _changeTracker = changeTracker;
            _streamNameProvider = streamNameProvider;
        }

        public void Add(TAggregateRoot root, string identifier = default) => AddAsync(root, identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task AddAsync(TAggregateRoot root, string identifier = default) =>
            this._changeTracker.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));
             
        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromChangeTrackerIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); } 

            var snapshotStreamName = _streamNameProvider.GetSnapshotStreamName(typeof(TAggregateRoot), identifier);

            Optional<Snapshot> snapshot = await _snapshotStore.GetLastSnapshot(snapshotStreamName);

            var version = StreamPosition.Start;

            if (snapshot.HasValue)
                version = snapshot.Value.Version + 1;

            var aggregate = await _aggregateStore.Load<TAggregateRoot>(identifier, version);

            AttachAggregateToChangeTracker(aggregate);

            return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
        }

        private Optional<Aggregate> GetAggregateFromChangeTrackerIfExits(string identifier)
        {
            Aggregate existingAggregate;

            _changeTracker.TryGet(identifier, out existingAggregate);

            return new Optional<Aggregate>(existingAggregate);
        }
        private void AttachAggregateToChangeTracker(Aggregate aggregate) =>
            this._changeTracker.Attach(aggregate);
    }
}
