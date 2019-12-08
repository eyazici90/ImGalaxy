using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public abstract class ImGalaxyIntegrationTestBase : IDisposable
    {
        protected IServiceProvider ServiceProvider { get; private set; }

        public ImGalaxyIntegrationTestBase()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            Configure(ServiceProvider);
        }
        protected async Task<IExecutionResult> WithUnitOfWorkAsync(Func<Task> funct)
        {
            var uow = ServiceProvider.GetRequiredService<IUnitOfWork>();

            await funct();

            return await uow.SaveChangesAsync();
        }
        protected abstract IServiceCollection ConfigureServices(IServiceCollection services);

        protected virtual void Configure(IServiceProvider app)
        { 
        }

        protected virtual T GetService<T>() =>
            ServiceProvider.GetService<T>();

        protected virtual T The<T>() =>
            ServiceProvider.GetService<T>(); 

        protected virtual T GetRequiredService<T>() =>
            ServiceProvider.GetRequiredService<T>();


        public virtual void Dispose()
        {
            ServiceProvider = null;
        }
    }
}
