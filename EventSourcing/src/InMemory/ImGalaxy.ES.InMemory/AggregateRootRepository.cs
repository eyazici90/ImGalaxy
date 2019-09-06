﻿using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{
    public class AggregateRootRepository<TAggregateRoot> : AggregateRootRepositoryBase<TAggregateRoot>, IAggregateRootRepository<TAggregateRoot>
           where TAggregateRoot : IAggregateRoot
    {
        public AggregateRootRepository(IChangeTracker changeTracker,
            IInMemoryConnection connection)
            : base(changeTracker, connection)
        {
        }

        public void Add(TAggregateRoot root, string identifier) =>
            root.With(r => ChangeTracker.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, r)));

        public async Task AddAsync(TAggregateRoot root, string identifier) =>
            root.With(r => ChangeTracker.Attach(new Aggregate(identifier, (int)ExpectedVersion.NoStream, r)));

        public Optional<TAggregateRoot> Get(string identifier) => GetAsync(identifier).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Optional<TAggregateRoot>> GetAsync(string identifier)
        {
            Optional<Aggregate> existingAggregate = GetAggregateFromChangeTrackerIfExits(identifier);

            if (existingAggregate.HasValue) { return new Optional<TAggregateRoot>((TAggregateRoot)existingAggregate.Value.Root); }

            var streamName = GetStreamNameOfRoot(identifier);

            var version = 0;

            var slice = await ReadStreamEventsForwardAsync(streamName, version);

            if (!slice.HasValue) { return Optional<TAggregateRoot>.Empty; }

            TAggregateRoot root = IntanceOfRoot().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice.Value));

            ClearChangesOfRoot(root);

            AttachAggregateToChangeTracker(identifier, (int)slice.Value.LastEventNumber, root);

            return new Optional<TAggregateRoot>(root);
        }


    }
}
