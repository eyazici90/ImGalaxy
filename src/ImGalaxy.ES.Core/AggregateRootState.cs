using Galaxy.Railway;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRootState<TState> : StateBase<TState>, IAggregateRootState<TState>, IAggregateRoot
        where TState : class 
    {  
        private readonly IEventRecorder _eventRecorder;
        private IReadOnlyCollection<object> _events => _eventRecorder?.RecordedEvents;

        public AggregateRootState() => _eventRecorder ??= new EventRecorder();

        public Result ApplyEvent(params object[] @events)
        {
            var state = this as TState;
            @events.ForEach(@event => ApplyEvent(@event));
            return new Result(state, _events);
        }

        private void ApplyEvent(object @event)
        {
            @event.ThrowsIfNull(new ArgumentNullException(nameof(@event))); 
            BeforeApplyEvent(@event);
            Play(@event);
            RecordEvent(@event);
            AfterApplyEvent(@event);
        }

        public void ApplyAllEvents() => _events.ForEach(@event => Play(@event));

        public bool HasEvents() => _events.Any();

        public IEnumerable<object> GetEvents() => _events.AsEnumerable();

        private void RecordEvent(object eventItem) => _eventRecorder.Record(eventItem);

        public void ClearEvents() => _eventRecorder?.Reset();

        public void Initialize(IEnumerable<object> events) => events.ForEach(e=> ApplyEvent(e)); 
         
        public virtual string GetStreamName(string id) => $"{typeof(TState).FullName}-{id}";            
    }
 
}
