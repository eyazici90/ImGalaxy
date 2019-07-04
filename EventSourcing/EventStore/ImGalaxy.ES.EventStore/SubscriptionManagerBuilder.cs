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
        private int? _maxLiveQueueSize;
        private ProjectionHandler[] _projections;
        private int? _readBatchSize;
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
            _maxLiveQueueSize = maxLiveQueueSize;
            return this;
        }

        public SubscriptionManagerBuilder CheckpointStore(ICheckpointStore checkpointStore)
        {
            _checkpointStore = checkpointStore;
            return this;
        }

        public SubscriptionManagerBuilder ReadBatchSize(int readBatchSize)
        {
            _readBatchSize = readBatchSize;
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

        //public SubscriptionManager Build() =>
        //  new SubscriptionManager(_connection, _deserializer, _checkpointStore, _projections, _snapshotters, _maxLiveQueueSize, _readBatchSize);

        //public async Task<SubscriptionManager> Activate()
        //{
        //    SubscriptionManager manager = Build();
        //    await manager.Activate();
        //    return manager;
        //}


    }
}
