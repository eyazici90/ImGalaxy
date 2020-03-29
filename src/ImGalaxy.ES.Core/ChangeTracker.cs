using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class ChangeTracker : IChangeTracker
    {
        private readonly ConcurrentDictionary<string, Aggregate> _aggregates;
        public ChangeTracker()=>
            _aggregates = new ConcurrentDictionary<string, Aggregate>();
        
        public IExecutionResult Attach(Aggregate aggregate)
        {
            aggregate.ThrowsIfNull(new ArgumentNullException(nameof(aggregate)));

            aggregate.ThrowsIf(a => !_aggregates.TryAdd(aggregate.Identifier, aggregate),
                new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                        $@"The aggregate of type '{aggregate.Root.GetType().Name}' with identifier '{aggregate.Identifier}' was already added.")));

            return ExecutionResult.Success;
        }

        public IEnumerable<Aggregate> GetChanges()=>
            _aggregates.Values.Where(aggregate => aggregate.Root.HasEvents());

        public bool HasChanges()=>
               _aggregates.Values.Any(aggregate => aggregate.Root.HasEvents());

        public IExecutionResult ResetChanges()
        {
            _aggregates.Clear();
            return ExecutionResult.Success;
        }
                
        public bool TryGet(string identifier, out Aggregate aggregate) => _aggregates.TryGetValue(identifier, out aggregate);
    }
}
