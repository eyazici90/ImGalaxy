using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExpectedVersion = ImGalaxy.ES.Core.ExpectedVersion;

namespace ImGalaxy.ES.EventStore
{
    public class SnapshotEventStore<TAggregateRoot, TSnapshot> : ISnapshotter
        where TAggregateRoot : IAggregateRoot, ISnapshotable
    {

        private readonly IAggregateRootRepository<TAggregateRoot> _rootRepository;
        private readonly IChangeTracker _changeTracker;
        private readonly IStreamNameProvider _streamNameProvider;
        private readonly IEventStoreConnection _connection;
        private readonly IEventSerializer _eventSerializer; 
        private readonly Func<ResolvedEvent, bool> _strategy;
        public SnapshotEventStore(IAggregateRootRepository<TAggregateRoot> rootRepository,
            IChangeTracker changeTracker,
            IStreamNameProvider streamNameProvider,
            IEventStoreConnection connection,
            IEventSerializer eventSerializer, 
            Func<ResolvedEvent, bool> strategy)
        {
            _rootRepository = rootRepository ?? throw new ArgumentNullException(nameof(rootRepository));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer)); 
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

      
        public bool ShouldTakeSnapshot(Type aggregateType, object @event) =>
                typeof(ISnapshotable).IsAssignableFrom(aggregateType) && _strategy((ResolvedEvent)@event);

       
        public async Task<IExecutionResult> TakeSnapshotAsync(string stream)
        {
            Optional<TAggregateRoot> root = await _rootRepository.GetAsync(stream);

            root.ThrowsIf(r=>!r.HasValue, new AggregateNotFoundException(stream));

            Aggregate aggregate;

            this._changeTracker.TryGet(stream, out aggregate); 

            var changes = new EventData(
                                        Guid.NewGuid(),
                                        typeof(TSnapshot).TypeQualifiedName(),
                                        true,
                                        Encoding.UTF8.GetBytes(this._eventSerializer.Serialize(((ISnapshotable)root.Value).TakeSnapshot())),
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

            return ExecutionResult.Success;
        }
        
    }
}
