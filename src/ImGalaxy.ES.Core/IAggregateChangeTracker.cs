using System.Collections.Generic; 

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
