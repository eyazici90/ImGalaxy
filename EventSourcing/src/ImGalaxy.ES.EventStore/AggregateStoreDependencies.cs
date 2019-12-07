using EventStore.ClientAPI;
using ImGalaxy.ES.Core; 

namespace ImGalaxy.ES.EventStore
{
    public class AggregateStoreDependencies : IAggregateStoreDependencies
    { 
        public IEventDeserializer EventDeserializer { get; }
        public IEventStoreConnection EventStoreConnection { get; }
        public IEventStoreConfigurations EventStoreConfigurations { get; }
        public IStreamNameProvider StreamNameProvider { get; }
        public IEventSerializer EventSerializer { get; }

        public AggregateStoreDependencies(IEventDeserializer eventDeserializer,
            IEventStoreConnection eventStoreConnection,
            IEventStoreConfigurations eventStoreConfigurations,
            IStreamNameProvider streamNameProvider,
            IEventSerializer eventSerializer)
        {
            EventDeserializer = eventDeserializer;
            EventStoreConnection = eventStoreConnection;
            EventStoreConfigurations = eventStoreConfigurations;
            StreamNameProvider = streamNameProvider;
            EventSerializer = eventSerializer;
        }
    }
}
