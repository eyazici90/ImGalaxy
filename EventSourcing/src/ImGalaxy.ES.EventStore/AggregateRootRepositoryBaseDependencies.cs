using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public class AggregateRootRepositoryBaseDependencies : IAggregateRootRepositoryBaseDependencies
    {
        public IChangeTracker ChangeTracker { get; }
        public IEventDeserializer EventDeserializer { get; }
        public IEventStoreConnection EventStoreConnection { get; }
        public IEventStoreConfigurations EventStoreConfigurations { get; }
        public IStreamNameProvider StreamNameProvider { get; }
        public AggregateRootRepositoryBaseDependencies(IChangeTracker changeTracker,
            IEventDeserializer eventDeserializer,
            IEventStoreConnection eventStoreConnection,
            IEventStoreConfigurations eventStoreConfigurations,
            IStreamNameProvider streamNameProvider)
        {
            ChangeTracker = changeTracker;
            EventDeserializer = eventDeserializer;
            EventStoreConnection = eventStoreConnection;
            EventStoreConfigurations = eventStoreConfigurations;
            StreamNameProvider = streamNameProvider;
        }
    }
}
