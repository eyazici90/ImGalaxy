using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory.Modules
{
    public static class ImGalaxyESInMemoryModule
    {
        public static IServiceCollection AddImGalaxyESInMemoryModule(this IServiceCollection services) =>
               services.With(s =>
               {
                   s.RegisterProviders()
                    .RegisterChangeTracker()
                    .RegisterInMemoryConnection()
                    .RegisterRepositories() 
                    .RegisterUnitOfWork();
               });
         
        private static IServiceCollection RegisterProviders(this IServiceCollection services) =>
                     services.AddTransient<ISnapshotReader, NullInstanceSnapshotReader>();
        private static IServiceCollection RegisterChangeTracker(this IServiceCollection services) =>
           services.AddScoped<IChangeTracker, ChangeTracker>();
        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
             services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));
        private static IServiceCollection RegisterInMemoryConnection(this IServiceCollection services) =>
         services.AddSingleton<IInMemoryConnection, InMemoryConnection>();

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();


    }
}
