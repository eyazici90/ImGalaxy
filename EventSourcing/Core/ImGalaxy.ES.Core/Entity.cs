using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class Entity : IEntity
    {
        private IEventRouter _eventRouter; 

        public Entity()
        {
            _eventRouter = _eventRouter ?? new EventRouter(); 
        }

        public void RegisterEvent<TEvent>(Action<TEvent> handler) =>
            _eventRouter.RegisterEvent(handler);

        public void RegisterEvent<TEvent>(Type @eventType, Action<object> handler) =>
           _eventRouter.RegisterEvent(eventType, handler);

        public void ApplyEvent(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            BeforeApplyChange(@event);
            Play(@event); 
            AfterApplyChange(@event);
        }
         

        private void Play(object @event) =>
            _eventRouter.Route(@event);


        public virtual void BeforeApplyChange(object @event)
        {
        }


        public virtual void AfterApplyChange(object @event)
        {
        } 
    }
}
