using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{ 
    public abstract class EntityState<TState> : StateBase<TState>, IEntityState<TState> where TState : class
    {
        public Result ApplyEvent(object @event)
        {
            @event.ThrowsIfNull(new ArgumentNullException(nameof(@event))); 
            BeforeApplyEvent(@event);
            Play(@event);
            AfterApplyEvent(@event);
            var newResult = new Result(this as TState, null);
            return newResult;
        } 
    }
}
