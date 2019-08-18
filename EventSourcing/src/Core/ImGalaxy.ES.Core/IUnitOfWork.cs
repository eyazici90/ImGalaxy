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
        IExecutionResult SaveChanges();
        Task<IExecutionResult> SaveChangesAsync();
        IEnumerable<Aggregate> GetChanges();
    }
}
