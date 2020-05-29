using System;
using System.Collections.Generic; 

namespace ImGalaxy.ES.Core
{
    public abstract class StateBase<TState> where TState : class
    {
        protected IEventRouter EventRouter { get; }

        public StateBase() => EventRouter ??= new EventRouter();

        public void RegisterEvent<TEvent>(Action<TEvent> handler) =>
            EventRouter.RegisterEvent(handler);

        public void RegisterEvent<TEvent>(Type @eventType, Action<object> handler) =>
           EventRouter.RegisterEvent(eventType, handler);

        public Result EmptyResult() => new Result(this as TState, new List<object>());

        public class Result
        {
            public TState State { get; }
            public IEnumerable<object> Events { get; }

            public Result(TState state, IEnumerable<object> events)
            {
                State = state;
                Events = events;
            }
        }

        public TState With(TState state, Action<TState> update)
        {
            update(state);
            return state;
        }
        public virtual void BeforeApplyEvent(object @event)
        {
        } 

        public virtual void AfterApplyEvent(object @event)
        {
        }

        protected void Play(object @event) => EventRouter.Route(@event);

    }
}
