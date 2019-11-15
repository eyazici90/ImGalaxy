using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class StateBase<TState>  where TState: class
    {
        protected readonly IEventRouter EventRouter;

        public StateBase() =>
            EventRouter = EventRouter ?? new EventRouter();
         
        public void RegisterEvent<TEvent>(Action<TEvent> handler) =>
            EventRouter.RegisterEvent(handler);

        public void RegisterEvent<TEvent>(Type @eventType, Action<object> handler) =>
           EventRouter.RegisterEvent(eventType, handler);

        public Result EmptyResult() => new Result(this as TState, new List<object>());
         

        public class Result
        {
            public readonly TState State;
            public readonly IEnumerable<object> Events;

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

        protected void Play(object @event) =>
            EventRouter.Route(@event);

    }
}
