using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore.Modules
{
    public static class ImGalaxyESEventStoreModule
    {
        public static IServiceCollection AddGalaxyESEventStoreModule(this IServiceCollection services, Action<IEventStoreConfigurator> configurations) =>
            services.With(s=> 
            {
                s.RegisterConfigurations(configurations)
                 .RegisterRepositories()
                 .RegisterSnapshotableRepositories()
                 .RegisterUnitOfWork();
            });
     

        private static IServiceCollection RegisterConfigurations(this IServiceCollection services, Action<IEventStoreConfigurator> configurations) =>
             services.AddSingleton<IEventStoreConfigurator>(provider => new EventStoreConfigurations().With(c=> configurations(c)));
        
        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));
         
        private static IServiceCollection RegisterSnapshotableRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(ISnapshotableRootRepository<>), typeof(SnapshotableRootRepository<>));

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, EventStoreUnitOfWork>();

    }

}
