using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRootState<TState> : StateBase<TState>, IAggregateRootState<TState> where TState : class, IAggregateRoot 
    {  
        private IEventRecorder _eventRecorder;
        private IReadOnlyCollection<object> _events => _eventRecorder?.RecordedEvents;

        public AggregateRootState() =>
            _eventRecorder = _eventRecorder ?? new EventRecorder(); 

        public Result ApplyEvent(object @event)
        {
            @event.ThrowsIfNull(new ArgumentNullException(nameof(@event))); 
            BeforeApplyEvent(@event);
            Play(@event);
            RecordEvent(@event);
            AfterApplyEvent(@event);
            var newResult = new Result(this as TState, _events);

            return newResult;
        }

        public void ApplyAllChanges() => _events.ForEach(@event => Play(@event));

        public bool HasChanges() => _events.Any();

        public IEnumerable<object> GetChanges() => _events.AsEnumerable();

        private void RecordEvent(object eventItem) => _eventRecorder.Record(eventItem);

        public void ClearChanges() => _eventRecorder?.Reset();

        public void Initialize(IEnumerable<object> events) => events.ForEach(e=> ApplyEvent(e)); 
         
        public virtual string GetStreamName(string id) => $"{typeof(TState).FullName}-{id}";            
    }
 
}
