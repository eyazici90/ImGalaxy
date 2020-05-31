using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents; 
using System;  
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class SnapshotterCosmosDB<TAggregateRoot, TSnapshot> : ISnapshotter
       where TAggregateRoot : class, IAggregateRootState<TAggregateRoot>, IAggregateRoot, ISnapshotable
    {
        private readonly IAggregateRootRepository<TAggregateRoot> _rootRepository;
        private readonly IAggregateChangeTracker _changeTracker;
        private readonly ICosmosDBClient _cosmosDbClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        private readonly IEventSerializer _eventSerializer;
        public SnapshotterCosmosDB(IAggregateRootRepository<TAggregateRoot> rootRepository,
            IAggregateChangeTracker changeTracker,
            ICosmosDBClient cosmosDbClient,
            ICosmosDBConfigurations cosmosDBConfigurations,
            IEventSerializer eventSerializer)
        {
            _rootRepository = rootRepository ?? throw new ArgumentNullException(nameof(rootRepository));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _cosmosDbClient = cosmosDbClient ?? throw new ArgumentNullException(nameof(cosmosDbClient));
            _cosmosDBConfigurations = cosmosDBConfigurations ?? throw new ArgumentNullException(nameof(cosmosDBConfigurations));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }


        public bool ShouldTakeSnapshot(Type aggregateType, object @event) =>
            typeof(ISnapshotable).IsAssignableFrom(aggregateType)
                            && _cosmosDBConfigurations.SnapshotStrategy(@event as EventDocument);

        public async Task<IExecutionResult> TakeSnapshotAsync(string stream)
        {
            Optional<TAggregateRoot> root = await _rootRepository.GetAsync(stream);

            root.ThrowsIf(r => !r.HasValue, new AggregateNotFoundException(stream));
             
            _changeTracker.TryGet(stream, out Aggregate aggregate);

            var serializedState = _eventSerializer.Serialize(root.Value.TakeSnapshot());

            var newSnapshot = new SnapshotDocument(aggregate.Identifier, serializedState, aggregate.ExpectedVersion.Value.ToString(), null, typeof(TSnapshot).TypeQualifiedName());

            await _cosmosDbClient.CreateItemAsync(newSnapshot,
                             _cosmosDBConfigurations.SnapshotContainerName).ConfigureAwait(false);

            return ExecutionResult.Success;
        }


    }
}
