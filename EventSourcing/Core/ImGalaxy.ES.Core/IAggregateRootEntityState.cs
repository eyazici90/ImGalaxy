using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootEntityState<TState>
    {
        string GetStreamName(string id);

        TState With(TState state, Action<TState> update);
    }
}
