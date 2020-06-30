using Galaxy.Railway;  
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IUnitOfWork
    {
        IExecutionResult SaveChanges();
        Task<IExecutionResult> SaveChangesAsync(); 
    }
}
