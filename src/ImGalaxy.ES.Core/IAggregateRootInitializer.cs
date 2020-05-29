using System.Collections.Generic;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootInitializer
    { 
        void Initialize(IEnumerable<object> events);
    }
}
