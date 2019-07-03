using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IUnitOfWork
    {
         bool TryGet(string identifier, out Aggregate aggregate);
         void Attach(Aggregate aggregateRoot); 
         bool HasChanges(); 
         IEnumerable<Aggregate> GetChanges();
    }
}
