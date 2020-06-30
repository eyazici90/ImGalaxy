using Galaxy.Railway;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{
    public abstract class AggregateStoreBase
    {
        protected IInMemoryConnection Connection { get; }
        public AggregateStoreBase(IInMemoryConnection connection) =>
            Connection = connection;

        protected virtual T ApplyChangesToRoot<T>(T root, IEnumerable<object> events) where T : IAggregateRootState<T> =>
          root.With(r => (r as IAggregateRootInitializer).Initialize(events));
        protected virtual IEnumerable<object> DeserializeEventsFromSlice(InMemoryStream slice) =>
               slice.Events.Select(e => e.Data.Data);
        protected virtual string GetStreamNameOfRoot<T>(string identifier) => $"{typeof(T).Name}-{identifier}";
        protected virtual Optional<T> IntanceOfRoot<T>() where T : IAggregateRootState<T> =>
            ((T)Activator.CreateInstance(typeof(T), true)).ToOptional();
        protected virtual Optional<T> IntanceOfRoot<T>(Aggregate aggregate) where T : IAggregateRootState<T> =>
            ((T)((aggregate).Root)).ToOptional();
        protected virtual void ClearChangesOfRoot<T>(T root) where T : IAggregateRootState<T> => (root as IAggregateRootChangeTracker).ClearEvents();
        protected virtual async Task<Optional<InMemoryStream>> ReadStreamEventsForwardAsync(string streamName, long version) =>
          await Connection.ReadStreamEventsForwardAsync(streamName, version, int.MaxValue);
    }
}
