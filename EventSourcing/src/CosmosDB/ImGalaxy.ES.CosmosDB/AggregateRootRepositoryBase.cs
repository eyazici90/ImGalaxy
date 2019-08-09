using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public abstract class AggregateRootRepositoryBase<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IEventDeserializer EventDeserializer;
        protected readonly ICosmosDBConnection CosmosDBConnection;
        protected readonly ICosmosDBConfigurator CosmosDBConfigurator;
        protected readonly IStreamNameProvider StreamNameProvider;
        public AggregateRootRepositoryBase(IUnitOfWork unitOfWork,
            IEventDeserializer eventDeserializer,
            ICosmosDBConnection cosmosDBConnection,
            ICosmosDBConfigurator cosmosDBConfigurator,
            IStreamNameProvider streamNameProvider)
        {
            UnitOfWork = unitOfWork;
            EventDeserializer = eventDeserializer;
            CosmosDBConnection = cosmosDBConnection;
            CosmosDBConfigurator = cosmosDBConfigurator;
            StreamNameProvider = streamNameProvider;
        }
        protected virtual TAggregateRoot ApplyChangesToRoot(TAggregateRoot root, IEnumerable<object> events) =>
          root.With(r => (r as IAggregateRootInitializer).Initialize(events));
        protected virtual IEnumerable<object> DeserializeEventsFromSlice(CosmosStream slice) => slice.Events.AsEnumerable();
        protected virtual string GetStreamNameOfRoot(string identifier) => StreamNameProvider.GetStreamName(typeof(TAggregateRoot), identifier);
        protected virtual Optional<TAggregateRoot> IntanceOfRoot() => new Optional<TAggregateRoot>((TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), true));
        protected virtual Optional<TAggregateRoot> IntanceOfRoot(Aggregate aggregate) => new Optional<TAggregateRoot>((TAggregateRoot)((aggregate).Root));

        protected virtual Optional<Aggregate> GetAggregateFromUnitOfWorkIfExits(string identifier)
        {
            Aggregate existingAggregate;

            UnitOfWork.TryGet(identifier, out existingAggregate);

            return new Optional<Aggregate>(existingAggregate);
        }

        protected virtual void ClearChangesOfRoot(TAggregateRoot root) => root.ClearChanges();
        protected virtual void AttachAggregateToUnitOfWork(string identifier, int expectedVersion, TAggregateRoot aggregateRoot) =>
            aggregateRoot.With(root =>
            {
                var aggregate = new Aggregate(identifier, expectedVersion, root);
                this.UnitOfWork.Attach(aggregate);
            });

        protected virtual async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamName, long version) =>
              await CosmosDBConnection.ReadStreamEventsForwardAsync(streamName, version, this.CosmosDBConfigurator.ReadBatchSize);
    }
}
