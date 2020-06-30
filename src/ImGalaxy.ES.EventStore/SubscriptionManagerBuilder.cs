using EventStore.ClientAPI;
using Galaxy.Railway;
using ImGalaxy.ES.Core; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SubscriptionManagerBuilder
    {
        public static readonly SubscriptionManagerBuilder New = new SubscriptionManagerBuilder();
        private ICheckpointStore _checkpointStore;
        private IEventStoreConnection _connection;
        private IEventDeserializer _deserializer;
        private IEventStoreConfigurations _configurator;
        private ProjectionHandler[] _projections;
        private ISnapshotter[] _snapshotstore;

        public SubscriptionManagerBuilder Connection(IEventStoreConnection connection) =>
            this.With(t => _connection = connection);

        public SubscriptionManagerBuilder Deserializer(IEventDeserializer deserializer) =>
            this.With(t => _deserializer = deserializer);

        public SubscriptionManagerBuilder MaxLiveQueueSize(int maxLiveQueueSize) =>
             this.With(t => _configurator.MaxLiveQueueSize = maxLiveQueueSize);

        public SubscriptionManagerBuilder CheckpointStore(ICheckpointStore checkpointStore) =>
            this.With(t => _checkpointStore = checkpointStore);

        public SubscriptionManagerBuilder ReadBatchSize(int readBatchSize) =>
            this.With(t => _configurator.ReadBatchSize = readBatchSize);

        public SubscriptionManagerBuilder SnaphotStore(params ISnapshotter[] snapshotstore) =>
            this.With(t => _snapshotstore = snapshotstore);

        public SubscriptionManagerBuilder Projections(params ProjectionHandler[] projections) =>
            this.With(t => _projections = projections);

        public SubscriptionManager Build() =>
            new SubscriptionManager(_connection, _checkpointStore, _projections, _snapshotstore, _deserializer, _configurator);

        public async Task<SubscriptionManager> Activate() 
        {
            SubscriptionManager manager = Build();
            await manager.Activate();
            return manager;
        } 

    }
}
