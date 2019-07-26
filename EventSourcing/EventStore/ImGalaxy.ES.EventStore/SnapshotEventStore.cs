﻿using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class SnapshotEventStore<TAggregateRoot, TSnapshot> : ISnapshotStore
        where TAggregateRoot : IAggregateRoot, ISnapshotable<TSnapshot>
        where TSnapshot : class
    {

        private readonly ISnapshotableRootRepository<TAggregateRoot, TSnapshot> _snapRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStreamNameProvider _streamNameProvider;
        private readonly IEventStoreConnection _connection;
        private readonly IEventSerializer _eventSerializer;
        private readonly IEventDeserializer _deserializer;
        private readonly Func<ResolvedEvent, bool> _strategy;
        public SnapshotEventStore(ISnapshotableRootRepository<TAggregateRoot, TSnapshot> snapRepository,
            IUnitOfWork unitOfWork,
            IStreamNameProvider streamNameProvider,
            IEventStoreConnection connection,
            IEventSerializer eventSerializer,
            IEventDeserializer deserializer,
            Func<ResolvedEvent, bool> strategy)
        {
            _snapRepository = snapRepository ?? throw new ArgumentNullException(nameof(snapRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public async Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream)
        {
            if (snapshotStream == null) throw new ArgumentNullException(nameof(snapshotStream));

            var slice = await _connection.ReadStreamEventsBackwardAsync(snapshotStream, StreamPosition.End, 1, false);

            if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound ||
                (slice.Events.Length == 0 && slice.NextEventNumber == -1))
            {
                return Optional<Snapshot>.Empty;
            }
            var e = slice.Events[0].Event;
            var eData = this._deserializer.Deserialize(Type.GetType(e.EventType, true)
                                                        , Encoding.UTF8.GetString(e.Data));

            var eMetaData = this._deserializer.Deserialize<EventMetadata>(Encoding.UTF8.GetString(e.Metadata));

            return new Optional<Snapshot>(new Snapshot(eMetaData.Version, eData));
        }

        public bool ShouldTakeSnapshot(Type aggregateType, object @event) =>
                typeof(ISnapshotable<TSnapshot>).IsAssignableFrom(aggregateType) && _strategy((ResolvedEvent)@event);

        public async Task TakeSnapshot(string stream)
        {
            TAggregateRoot root = await _snapRepository.GetAsync(stream);

            Aggregate aggregate;

            this._unitOfWork.TryGet(stream, out aggregate);

            if (root == null) { throw new AggregateNotFoundException($"Aggregate not found by {stream}"); }

            var changes = new EventData(
                                        Guid.NewGuid(),
                                        typeof(TSnapshot).TypeQualifiedName(),
                                        true,
                                        Encoding.UTF8.GetBytes(this._eventSerializer.Serialize(((ISnapshotable<TSnapshot>)root).TakeSnapshot())),
                                        Encoding.UTF8.GetBytes(this._eventSerializer.Serialize(new EventMetadata
                                        {
                                            AggregateAssemblyQualifiedName = typeof(TAggregateRoot).AssemblyQualifiedName,
                                            AggregateType = typeof(TAggregateRoot).Name,
                                            TimeStamp = DateTime.Now,
                                            IsSnapshot = true,
                                            Version = aggregate.ExpectedVersion
                                        })
                                       ));
           var snapShotStreamName = _streamNameProvider.GetSnapshotStreamName(root, stream);
           await _connection.AppendToStreamAsync(snapShotStreamName, ExpectedVersion.Any, changes);
        }
    }
}
