﻿using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public abstract class AggregateRootRepositoryBase<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IEventDeserializer EventDeserializer;
        protected readonly IEventStoreConnection EventStoreConnection;
        protected readonly IEventStoreConfigurator EventStoreConfigurator;
        protected readonly IStreamNameProvider StreamNameProvider;
        public AggregateRootRepositoryBase(IUnitOfWork unitOfWork,
            IEventDeserializer eventDeserializer,
            IEventStoreConnection eventStoreConnection,
            IEventStoreConfigurator eventStoreConfigurator,
            IStreamNameProvider streamNameProvider)
        {
            UnitOfWork = unitOfWork;
            EventDeserializer = eventDeserializer;
            EventStoreConnection = eventStoreConnection;
            EventStoreConfigurator = eventStoreConfigurator;
            StreamNameProvider = streamNameProvider;
        }
        protected virtual TAggregateRoot ApplyChangesToRoot(TAggregateRoot root, IEnumerable<object> events) =>
          root.With(r => (r as IAggregateRootInitializer).Initialize(events)); 
        protected virtual IEnumerable<object> DeserializeEventsFromSlice(StreamEventsSlice slice) =>
            slice.Events.Select(e => this.EventDeserializer.Deserialize(Type.GetType(e.Event.EventType, true)
                       , Encoding.UTF8.GetString(e.Event.Data))); 
        protected virtual string GetStreamNameOfRoot(string identifier) => StreamNameProvider.GetStreamName(typeof(TAggregateRoot), identifier);
        protected virtual TAggregateRoot IntanceOfRoot() => (TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), true);
        protected virtual TAggregateRoot IntanceOfRoot(Aggregate aggregate) => (TAggregateRoot)((aggregate).Root);

        protected virtual Aggregate GetAggregateFromUnitOfWorkIfExits(string identifier)
        {
            Aggregate existingAggregate;

            UnitOfWork.TryGet(identifier, out existingAggregate);

            return existingAggregate;
        }

        protected virtual void ClearChangesOfRoot(TAggregateRoot root) => root.ClearChanges();
        protected virtual TAggregateRoot AttachAggregateToUnitOfWork(string identifier, int expectedVersion, TAggregateRoot aggregateRoot) =>
            aggregateRoot.With(root =>
            {
                var aggregate = new Aggregate(identifier, expectedVersion, root);
                this.UnitOfWork.Attach(aggregate);
            });

        protected virtual async Task<StreamEventsSlice> ReadStreamEventsForwardAsync(string streamName, long version) =>
              await EventStoreConnection.ReadStreamEventsForwardAsync(streamName, version, this.EventStoreConfigurator.ReadBatchSize, false);
    }
}
