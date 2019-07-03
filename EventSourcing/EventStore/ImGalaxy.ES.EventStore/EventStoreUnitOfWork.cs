using ImGalaxy.ES.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public class EventStoreUnitOfWork : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, Aggregate> _aggregates;

        public EventStoreUnitOfWork() => _aggregates = new ConcurrentDictionary<string, Aggregate>(); 
       
        public void Attach(Aggregate aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }
            if (!_aggregates.TryAdd(aggregate.Identifier, aggregate))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                        $@"The aggregate of type '{aggregate.Root.GetType().Name}' with identifier '{aggregate.Identifier}' was already added."));
            }
        }
         
        public bool TryGet(string identifier, out Aggregate aggregate) => _aggregates.TryGetValue(identifier, out aggregate);
 
        public bool HasChanges() =>
             _aggregates.Values.Any(aggregate => aggregate.Root.HasChanges());
        
        public IEnumerable<Aggregate> GetChanges() =>
             _aggregates.Values.Where(aggregate => aggregate.Root.HasChanges());
        
    }
}
