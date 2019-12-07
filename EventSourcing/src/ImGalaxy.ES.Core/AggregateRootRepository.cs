﻿using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public class AggregateRootRepository<TAggregateRoot> : IAggregateRootRepository<TAggregateRoot>
            where TAggregateRoot : class, IAggregateRootState<TAggregateRoot>, IAggregateRoot
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IChangeTracker _changeTracker;
        public AggregateRootRepository(IAggregateStore aggregateStore,
            IChangeTracker changeTracker)
        {
            _aggregateStore = aggregateStore;
            _changeTracker = changeTracker;
        }

        public void Add(TAggregateRoot root, string identifier) =>
            root.With(r => _changeTracker.Attach(new Aggregate(identifier, (int) ExpectedVersion.NoStream, r)));

        public async Task AddAsync(TAggregateRoot root, string identifier) =>
            root.With(r => _changeTracker.Attach(new Aggregate(identifier, (int) ExpectedVersion.NoStream, r)));

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {   
            Optional<Aggregate> existingAggregate = GetAggregateFromChangeTrackerIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); }

            var aggregate = await _aggregateStore.Load<TAggregateRoot>(identifier); 

            AttachAggregateToChangeTracker(aggregate);

            return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root); 
        }

        private Optional<Aggregate> GetAggregateFromChangeTrackerIfExits(string identifier)
        {
            Aggregate existingAggregate;

            _changeTracker.TryGet(identifier, out existingAggregate);

            return new Optional<Aggregate>(existingAggregate);
        } 
        private void AttachAggregateToChangeTracker(Aggregate aggregate) =>
            this._changeTracker.Attach(aggregate);

    }
}
