using EventStore.ClientAPI;
using ImGalaxy.ES.Core; 

namespace ImGalaxy.ES.EventStore
{
    public interface IAggregateStoreDependencies
    {
        IEventSerializer EventSerializer { get; }
        IEventDeserializer EventDeserializer { get; }
        IEventStoreConnection EventStoreConnection { get; }
        IEventStoreConfigurations EventStoreConfigurations { get; }
        IStreamNameProvider StreamNameProvider { get; }
    }
}
