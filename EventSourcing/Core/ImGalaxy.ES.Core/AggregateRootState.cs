using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRootState<TState> : AggregateRoot, IAggregateRootState<TState> 
            where TState : IAggregateRoot
    {
        public TState With(TState state, Action<TState> update)
        {
            update(state);
            return state;
        }
        public virtual string GetStreamName(string id) => $"{typeof(TState).FullName}-{id}";            
    }
 
}
