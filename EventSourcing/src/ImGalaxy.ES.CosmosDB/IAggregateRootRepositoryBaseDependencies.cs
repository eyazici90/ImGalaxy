using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public interface IAggregateRootRepositoryBaseDependencies
    {
        IEventDeserializer EventDeserializer { get; }
        IChangeTracker ChangeTracker { get; }
        ICosmosDBConnection CosmosDBConnection { get; }
        ICosmosDBConfigurations CosmosDBConfigurator { get; }
        IStreamNameProvider StreamNameProvider { get; }
    }
}
