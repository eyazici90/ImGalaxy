using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IUnitOfWork
    {
        IExecutionResult SaveChanges();
        Task<IExecutionResult> SaveChangesAsync(); 
    }
}
