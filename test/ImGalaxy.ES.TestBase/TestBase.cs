using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.TestBase
{
    public abstract class TestBase : IDisposable
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IServiceCollection ServiceCollection { get; private set; }
        public TestBase()
        {
            ServiceCollection = new ServiceCollection();

            ConfigureServices(ServiceCollection);

        }

        protected virtual IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        protected virtual void Configure(IServiceProvider app)
        {
        }

        protected virtual T GetService<T>()
        {
            EnsureContainerInitialized();
            return ServiceProvider.GetService<T>();
        }


        protected virtual T The<T>() =>
                 GetService<T>();

        protected virtual IServiceCollection SetThe<T>() where T : class =>
            ServiceCollection.AddTransient<T>();

        protected virtual IServiceCollection UseThe<T>(T valueToSet) where T : class =>
             ServiceCollection.AddTransient(provider => valueToSet);

        protected void EnsureContainerInitialized()
        {
            if (ServiceProvider == null)
            {
                InitializeContainer();
            }
        }

        private void InitializeContainer()
        {
            ServiceProvider = ServiceCollection.BuildServiceProvider();
            Configure(ServiceProvider);
        }


        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServiceProvider = null;
            }
        }
    }
}
