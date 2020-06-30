using Galaxy.Railway;
using System; 

namespace ImGalaxy.ES.Core
{ 
    public abstract class EntityState<TState> : StateBase<TState>, IEntityState<TState> where TState : class
    {
        public Result ApplyEvent(params object[] @events)
        {
            var state = this as TState;
            @events.ForEach(e => ApplyEvent(e));
            return new Result(state, null);
        }

        private void ApplyEvent(object @event)
        {
            @event.ThrowsIfNull(new ArgumentNullException(nameof(@event))); 
            BeforeApplyEvent(@event);
            Play(@event);
            AfterApplyEvent(@event); 
        }
    }
}
