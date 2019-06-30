using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IEventRouter 
    {
        void RegisterEvent<TEvent>(Action<TEvent> handler); 
        void RegisterEvent(Type @event, Action<object> handler);
        void Route(object @event); 
        void Apply(object @event);
    }
}
