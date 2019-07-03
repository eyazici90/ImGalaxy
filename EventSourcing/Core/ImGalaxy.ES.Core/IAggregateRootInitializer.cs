using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootInitializer
    { 
        void Initialize(IEnumerable<object> events);
    }
}
