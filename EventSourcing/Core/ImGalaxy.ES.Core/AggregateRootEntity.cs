using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRootEntity : IAggregateRoot
    {
        private IEventRouter _eventRouter;

        private List<object> _events;

        public IReadOnlyCollection<object> Events => _events?.AsReadOnly();

        public AggregateRootEntity()
        {
            _eventRouter = _eventRouter ?? new EventRouter();
            _events = _events ?? new List<object>();
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
            AddEvent(@event);
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

        private void AddEvent(object eventItem) =>
            _events.Add(eventItem);

        public void ClearChanges() => _events?.Clear();
    }
}
