using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRootEntityState<TState> : AggregateRootEntity, IAggregateRootEntityState<TState> 
            where TState : class
    {
        public abstract string GetStreamName(string id); 
         
        public TState With(TState state, Action<TState> update)
        {
            update(state);
            return state;
        }
    }
}
