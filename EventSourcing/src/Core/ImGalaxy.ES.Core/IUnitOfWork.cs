using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IUnitOfWork
    {
        bool TryGet(string identifier, out Aggregate aggregate);
        void Attach(Aggregate aggregateRoot); 
        bool HasChanges();
        void SaveChanges();
        Task SaveChangesAsync();
        IEnumerable<Aggregate> GetChanges();
    }
}
