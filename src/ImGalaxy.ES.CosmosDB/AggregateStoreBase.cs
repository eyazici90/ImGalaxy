using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public abstract class AggregateStoreBase
    {
        protected IEventDeserializer EventDeserializer { get; } 
        protected ICosmosDBConnection CosmosDBConnection { get; }
        protected ICosmosDBConfigurations CosmosDBConfigurations { get; }
        protected IStreamNameProvider StreamNameProvider { get; }
        public AggregateStoreBase(IAggregateStoreDependencies dependencies)
        {
            EventDeserializer = dependencies.EventDeserializer; 
            CosmosDBConnection = dependencies.CosmosDBConnection;
            CosmosDBConfigurations = dependencies.CosmosDBConfigurations;
            StreamNameProvider = dependencies.StreamNameProvider;
        }

        protected virtual T ApplyChangesToRoot<T>(T root, IEnumerable<object> events) where T : IAggregateRootState<T> =>
          root.With(r => (r as IAggregateRootInitializer).Initialize(events));
        protected virtual IEnumerable<object> DeserializeEventsFromSlice(CosmosStream slice) =>
            slice.Events.Select(e => this.EventDeserializer.Deserialize(Type.GetType(e.EventType, true), e.Data));
        protected virtual string GetStreamNameOfRoot<T>(string identifier) => StreamNameProvider.GetStreamName(typeof(T), identifier);
        protected virtual Optional<T> IntanceOfRoot<T>() where T : IAggregateRootState<T> =>
        new Optional<T>((T)Activator.CreateInstance(typeof(T), true));
        protected virtual Optional<T> IntanceOfRoot<T>(Aggregate aggregate) where T : IAggregateRootState<T> =>
            new Optional<T>((T)((aggregate).Root)); 
        protected virtual void ClearChangesOfRoot<T>(T root) where T : IAggregateRootState<T> => (root as IAggregateChangeTracker).ClearEvents(); 
        protected virtual async Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamName, long version) =>
              await CosmosDBConnection.ReadStreamEventsForwardAsync(streamName, version, this.CosmosDBConfigurations.ReadBatchSize);
    }
}
