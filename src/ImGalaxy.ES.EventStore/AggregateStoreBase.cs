using EventStore.ClientAPI;
using Galaxy.Railway;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public abstract class AggregateStoreBase
    {
        protected IEventDeserializer EventDeserializer { get; }
        protected IEventStoreConnection EventStoreConnection { get; }
        protected IEventStoreConfigurations EventStoreConfigurations { get; }
        protected IStreamNameProvider StreamNameProvider { get; }
        public AggregateStoreBase(IAggregateStoreDependencies dependencies)
        {
            EventDeserializer = dependencies.EventDeserializer;
            EventStoreConnection = dependencies.EventStoreConnection;
            EventStoreConfigurations = dependencies.EventStoreConfigurations;
            StreamNameProvider = dependencies.StreamNameProvider;
        }
        protected virtual T ApplyChangesToRoot<T>(T root, IEnumerable<object> events) where T : IAggregateRootState<T> =>
          root.With(r => (r as IAggregateRootInitializer).Initialize(events));
        protected virtual IEnumerable<object> DeserializeEventsFromSlice(StreamEventsSlice slice) =>
            slice.Events.Select(e => this.EventDeserializer.Deserialize(Type.GetType(e.Event.EventType, true)
                       , Encoding.UTF8.GetString(e.Event.Data)));
        protected virtual string GetStreamNameOfRoot<T>(string identifier) where T : IAggregateRootState<T> =>
            StreamNameProvider.GetStreamName(typeof(T), identifier);
        protected virtual Optional<T> IntanceOfRoot<T>() where T : IAggregateRootState<T> =>
            ((T)Activator.CreateInstance(typeof(T), true)).ToOptional();
        protected virtual Optional<T> IntanceOfRoot<T>(Aggregate aggregate) where T : IAggregateRootState<T> =>
            ((T)((aggregate).Root)).ToOptional();

        protected virtual void ClearChangesOfRoot<T>(T root) where T : IAggregateRootState<T> => (root as IAggregateRootChangeTracker).ClearEvents();

        protected virtual async Task<StreamEventsSlice> ReadStreamEventsForwardAsync(string streamName, long version) =>
              await EventStoreConnection.ReadStreamEventsForwardAsync(streamName, version, this.EventStoreConfigurations.ReadBatchSize, false);
    }
}
