using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class EventRouter : IEventRouter
    {
        private readonly Dictionary<Type, Action<object>> _handlers; 
        public EventRouter() => _handlers = new Dictionary<Type, Action<object>>();

        private void ConfigureRoute(Type @event, Action<object> handler)
        {
            @event.ThrowsIfNull(new ArgumentNullException(nameof(@event)));

            handler.ThrowsIfNull(new ArgumentNullException(nameof(handler)));

            _handlers.Add(@event, handler);
        }

        private void ConfigureRoute<TEvent>(Action<TEvent> handler)
        {
            handler.ThrowsIfNull(new ArgumentNullException(nameof(handler)));

            _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        public void RegisterEvent<TEvent>(Action<TEvent> handler) => this.ConfigureRoute(handler);

        public void RegisterEvent(Type @event, Action<object> handler) => this.ConfigureRoute(@event, handler);

        public void Route(object @event)
        {
            @event.ThrowsIfNull(new ArgumentNullException(nameof(@event)));

            Action<object> handler;

            if (_handlers.TryGetValue(@event.GetType(), out handler))
                handler(@event);
            
        }

        public void Apply(object @event) => this.Route(@event);
    }
}
