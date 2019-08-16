using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateChangeTracker
    { 
        bool HasEvents(); 
        IEnumerable<object> GetEvents(); 
        void ClearEvents();
        void ApplyAllEvents();
    }
}
