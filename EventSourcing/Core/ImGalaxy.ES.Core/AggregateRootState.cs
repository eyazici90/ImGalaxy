using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class AggregateRootState<TState> : EntityState<TState>, IAggregateRootState<TState> 
            where TState : class
    {
        public abstract string GetStreamName(string id);  
    }
}
