using System;
using System.Collections.Generic;
using System.Text;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB
{
    public class AggregateRootRepositoryBaseDependencies : IAggregateRootRepositoryBaseDependencies
    {
        public IEventDeserializer EventDeserializer { get; }
        public IChangeTracker ChangeTracker { get; }
        public ICosmosDBConnection CosmosDBConnection { get; }
        public ICosmosDBConfigurations CosmosDBConfigurator { get; }
        public IStreamNameProvider StreamNameProvider { get; }
        public AggregateRootRepositoryBaseDependencies(IEventDeserializer eventDeserializer,
            IChangeTracker changeTracker,
            ICosmosDBConnection cosmosDBConnection,
            ICosmosDBConfigurations cosmosDBConfigurator,
            IStreamNameProvider streamNameProvider)
        {
            EventDeserializer = eventDeserializer;
            ChangeTracker = changeTracker;
            CosmosDBConnection = cosmosDBConnection;
            CosmosDBConfigurator = cosmosDBConfigurator;
            StreamNameProvider = streamNameProvider;
        }
    }
}
