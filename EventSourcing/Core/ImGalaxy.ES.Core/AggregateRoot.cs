using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        private IEventRouter _eventRouter;
        private IEventRecorder _eventRecorder;  
        public IReadOnlyCollection<object> Events => _eventRecorder?.RecordedEvents;

        public AggregateRoot()
        {
            _eventRouter = _eventRouter ?? new EventRouter();
            _eventRecorder = _eventRecorder ?? new EventRecorder();
        } 
        
        public  void RegisterEvent<TEvent>(Action<TEvent> handler) =>
            _eventRouter.RegisterEvent(handler);

        public  void RegisterEvent<TEvent>(Type @eventType, Action<object> handler) =>
           _eventRouter.RegisterEvent(eventType, handler);

        public void ApplyEvent(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            BeforeApplyChange(@event);
            Play(@event);
            RecordEvent(@event);
            AfterApplyChange(@event);
        }

        public void ApplyAllChanges()
        {
            foreach (var @event in Events)
            {
                _eventRouter.Route(@event);
            }
        }

        private void Play(object @event) =>
            _eventRouter.Route(@event);
         
        public virtual void BeforeApplyChange(object @event)
        {
        }
         
        public virtual void AfterApplyChange(object @event)
        {
        }

        public bool HasChanges() => Events.Any();

        public IEnumerable<object> GetChanges() => Events.AsEnumerable();

        private void RecordEvent(object eventItem) =>
            _eventRecorder.Record(eventItem);

        public void ClearChanges() => _eventRecorder?.Reset();

        public void Initialize(IEnumerable<object> events)
        {
            foreach (var e in events)
            {
                ApplyEvent(e);
            }
        }
    }
}
