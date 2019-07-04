using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SubscriptionManagerBuilder
    {
        public static readonly SubscriptionManagerBuilder New = new SubscriptionManagerBuilder();
        private ICheckpointStore _checkpointStore;
        private IEventStoreConnection _connection;
        private IEventDeserializer _deserializer; 
        private IEventStoreConfigurator _configurator;
        private ProjectionHandler[] _projections; 
        private ISnapshotStore[] _snapshotstore = { };

        public SubscriptionManagerBuilder Connection(IEventStoreConnection connection)
        {
            _connection = connection;
            return this;
        }

        public SubscriptionManagerBuilder Deserializer(IEventDeserializer deserializer)
        {
            _deserializer = deserializer;
            return this;
        }

        public SubscriptionManagerBuilder MaxLiveQueueSize(int maxLiveQueueSize)
        {
            _configurator.MaxLiveQueueSize = maxLiveQueueSize;
            return this;
        }

        public SubscriptionManagerBuilder CheckpointStore(ICheckpointStore checkpointStore)
        {
            _checkpointStore = checkpointStore;
            return this;
        }

        public SubscriptionManagerBuilder ReadBatchSize(int readBatchSize)
        {
            _configurator.ReadBatchSize = readBatchSize;
            return this;
        }

        public SubscriptionManagerBuilder SnaphotStore(params ISnapshotStore[] snapshotstore)
        {
            _snapshotstore = snapshotstore;
            return this;
        }

        public SubscriptionManagerBuilder Projections(params ProjectionHandler[] projections)
        {
            _projections = projections;
            return this;
        }

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
