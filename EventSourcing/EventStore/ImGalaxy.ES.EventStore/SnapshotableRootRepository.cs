using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SnapshotableRootRepository<TAggregateRoot> : ISnapshotableRootRepository<TAggregateRoot>
             where TAggregateRoot : IAggregateRoot, ISnapshotable
    {
        private readonly ISnapshotStore _snapshotStore;
        private readonly IStreamNameProvider _streamNameProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventDeserializer _eventDeserializer;
        private readonly IEventStoreConnection _connection;
        public SnapshotableRootRepository(ISnapshotStore snapshotStore,
            IStreamNameProvider streamNameProvider,
            IUnitOfWork unitOfWork,
            IEventStoreConnection connection,
            IEventDeserializer eventDeserializer)
        {
            _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _eventDeserializer = eventDeserializer ?? throw new ArgumentNullException(nameof(eventDeserializer));
        }
        public TAggregateRoot Add(TAggregateRoot root, string identifier = default)
        {
            this._unitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));
            return root;
        }

        public async Task<TAggregateRoot> AddAsync(TAggregateRoot root, string identifier = default)
        {
            this._unitOfWork.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, root));
            return root;
        }

        public TAggregateRoot Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<TAggregateRoot> GetAsync(string identifier)
        {
            Aggregate existingAggregate;

            _unitOfWork.TryGet(identifier, out existingAggregate);

            if (existingAggregate != null) { return (TAggregateRoot)((existingAggregate).Root); }

            TAggregateRoot root = (TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), true);

            var streamName = _streamNameProvider.GetStreamName(root, identifier);

            var snapshotStreamName = _streamNameProvider.GetSnapshotStreamName(root, identifier);

            Optional<Snapshot> snapshot = await _snapshotStore.GetLastSnapshot(snapshotStreamName);

            var version = StreamPosition.Start;

            if (snapshot.HasValue) { version = snapshot.Value.Version + 1; }

            StreamEventsSlice slice =
                 await
                     _connection.ReadStreamEventsForwardAsync(streamName, version, 100, false);

            if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) { throw new AggregateNotFoundException($"Aggregate not found by {streamName}"); }

            if (snapshot.HasValue)
            {
                root.RestoreSnapshot(snapshot.Value.State);
            }

            (root as IAggregateRootInitializer).Initialize(slice.Events.Select(e => this._eventDeserializer.Deserialize(Type.GetType(e.Event.EventType, true)
                        , Encoding.UTF8.GetString(e.Event.Data))));


            while (!slice.IsEndOfStream)
            {
                slice =
                    await
                        _connection.ReadStreamEventsForwardAsync(streamName, slice.NextEventNumber, 100,
                            false);

                (root as IAggregateRootInitializer).Initialize(slice.Events.Select(e => this._eventDeserializer.Deserialize(Type.GetType(e.Event.EventType, true)
                       , Encoding.UTF8.GetString(e.Event.Data))));
            }

           (root as IAggregateChangeTracker).ClearChanges();

            var aggregate = new Aggregate(identifier, (int)slice.LastEventNumber, root);

            this._unitOfWork.Attach(aggregate);

            return root;
        }

        public async Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
