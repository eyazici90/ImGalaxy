using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{ 
    public abstract class AggregateRootRepositoryBase<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        protected readonly IChangeTracker ChangeTracker;
        protected readonly IInMemoryConnection Connection;  
        public AggregateRootRepositoryBase(IChangeTracker changeTracker,
            IInMemoryConnection connection)
        { 
            ChangeTracker = changeTracker;
            Connection = connection; 
        }
        protected virtual TAggregateRoot ApplyChangesToRoot(TAggregateRoot root, IEnumerable<object> events) =>
          root.With(r => (r as IAggregateRootInitializer).Initialize(events));
        protected virtual IEnumerable<object> DeserializeEventsFromSlice(InMemoryStream slice) =>
            slice.Events.Select(e => e.Data);
        protected virtual string GetStreamNameOfRoot(string identifier) => $"{typeof(TAggregateRoot).Name}-{identifier}";
        protected virtual Optional<TAggregateRoot> IntanceOfRoot() => new Optional<TAggregateRoot>((TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), true));
        protected virtual Optional<TAggregateRoot> IntanceOfRoot(Aggregate aggregate) => new Optional<TAggregateRoot>((TAggregateRoot)((aggregate).Root));

        protected virtual Optional<Aggregate> GetAggregateFromChangeTrackerIfExits(string identifier)
        {
            Aggregate existingAggregate;

            ChangeTracker.TryGet(identifier, out existingAggregate);

            return new Optional<Aggregate>(existingAggregate);
        }

        protected virtual void ClearChangesOfRoot(TAggregateRoot root) => root.ClearEvents();
        protected virtual void AttachAggregateToChangeTracker(string identifier, int expectedVersion, TAggregateRoot aggregateRoot) =>
            aggregateRoot.With(root =>
            {
                var aggregate = new Aggregate(identifier, expectedVersion, root);
                this.ChangeTracker.Attach(aggregate);
            });

        protected virtual async Task<Optional<InMemoryStream>> ReadStreamEventsForwardAsync(string streamName, long version) =>
              await Connection.ReadStreamEventsForwardAsync(streamName, version, int.MaxValue);
    }
}
