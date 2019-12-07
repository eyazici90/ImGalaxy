using ImGalaxy.ES.Core; 

namespace ImGalaxy.ES.CosmosDB
{ 
    public interface IAggregateStoreDependencies
    {
        IEventSerializer EventSerializer { get; }
        IEventDeserializer EventDeserializer { get; }
        ICosmosDBConnection CosmosDBConnection { get; }
        ICosmosDBConfigurations CosmosDBConfigurations { get; }
        IStreamNameProvider StreamNameProvider { get; }
    }
}
