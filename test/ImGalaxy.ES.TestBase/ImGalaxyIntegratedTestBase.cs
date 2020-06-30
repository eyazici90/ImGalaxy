using Galaxy.Railway;
using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public abstract class ImGalaxyIntegratedTestBase : TestBase
    { 
        protected async Task<IExecutionResult> WithUnitOfWorkAsync(Func<Task> funct)
        {
            var uow = ServiceProvider.GetRequiredService<IUnitOfWork>();

            await funct();

            return await uow.SaveChangesAsync();
        } 
    }
}
