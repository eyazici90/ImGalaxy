using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class SnapshotterCosmosDB<TAggregateRoot, TSnapshot> : ISnapshotter
        where TAggregateRoot : IAggregateRoot, ISnapshotable
    {
        private readonly IAggregateRootRepository<TAggregateRoot> _rootRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICosmosDBClient _cosmosDbClient;
        private readonly ICosmosDBConfigurations _cosmosDBConfigurations;
        private readonly IEventSerializer _eventSerializer; 
        public SnapshotterCosmosDB(IAggregateRootRepository<TAggregateRoot> rootRepository,
            IUnitOfWork unitOfWork,
            ICosmosDBClient cosmosDbClient,
            ICosmosDBConfigurations cosmosDBConfigurations,
            IEventSerializer eventSerializer)
        {
            _rootRepository = rootRepository ?? throw new ArgumentNullException(nameof(rootRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

            Aggregate aggregate;

            this._unitOfWork.TryGet(stream, out aggregate);

            var serializedState = this._eventSerializer.Serialize(((ISnapshotable)root.Value).TakeSnapshot());

            var newSnapshot = new SnapshotDocument(aggregate.Identifier, serializedState, aggregate.ExpectedVersion.ToString(), null, typeof(TSnapshot).TypeQualifiedName());

            await _cosmosDbClient.CreateItemAsync(newSnapshot,
                            this._cosmosDBConfigurations.SnapshotCollectionName);

            return ExecutionResult.Success();
        }

       
    }
}
