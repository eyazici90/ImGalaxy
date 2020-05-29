using System;

namespace ImGalaxy.ES.Core
{ 
    public interface IEntityState<TState>
    {  
        TState With(TState state, Action<TState> update);
    }
}
