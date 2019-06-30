using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateChangeTracker
    {
        bool HasChanges(); 
        IEnumerable<object> GetChanges(); 
        void ClearChanges();
        void ApplyAllChanges();
    }
}
