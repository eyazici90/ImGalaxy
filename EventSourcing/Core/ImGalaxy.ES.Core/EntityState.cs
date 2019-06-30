using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{ 
    public abstract class EntityState<TState> : Entity, IEntityState<TState>
           where TState : class
    {  
        public TState With(TState state, Action<TState> update)
        {
            update(state);
            return state;
        }
    }
}
