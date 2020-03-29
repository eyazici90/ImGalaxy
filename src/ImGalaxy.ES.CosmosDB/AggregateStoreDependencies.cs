using System;
using System.Collections.Generic;
using System.Text;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB
{
    public class AggregateStoreDependencies : IAggregateStoreDependencies
    {
        public IEventSerializer EventSerializer { get; }
        public IEventDeserializer EventDeserializer { get; }
        public ICosmosDBConnection CosmosDBConnection { get; }
        public ICosmosDBConfigurations CosmosDBConfigurations { get; }
        public IStreamNameProvider StreamNameProvider { get; }
        public AggregateStoreDependencies(IEventSerializer eventSerializer,
            IEventDeserializer eventDeserializer,
            ICosmosDBConnection cosmosDBConnection,
            ICosmosDBConfigurations cosmosDBConfigurations,
            IStreamNameProvider streamNameProvider)
        {
            EventSerializer = eventSerializer;
            EventDeserializer = eventDeserializer;
            CosmosDBConnection = cosmosDBConnection;
            CosmosDBConfigurations = cosmosDBConfigurations;
            StreamNameProvider = streamNameProvider;
        }
    }
}
