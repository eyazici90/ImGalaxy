using EventStore.ClientAPI;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public interface IAggregateRootRepositoryBaseDependencies
    {
        IChangeTracker ChangeTracker { get; }
        IEventDeserializer EventDeserializer { get; }
        IEventStoreConnection EventStoreConnection { get; }
        IEventStoreConfigurations EventStoreConfigurations { get; }
        IStreamNameProvider StreamNameProvider { get; }
    }
}
