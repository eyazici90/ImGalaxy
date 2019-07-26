using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore.Modules
{
    public static class ImGalaxyESEventStoreModule
    {
        public static IServiceCollection AddGalaxyESEventStoreModule(this IServiceCollection services, Action<IEventStoreConfigurator> configurations)
        {
            RegisterConfigurations(services, configurations);
            RegisterRepositories(services);
            RegisterSnapshotableRepositories(services);
            RegisterUnitOfWork(services);
            return services; 
        }
        private static IServiceCollection RegisterConfigurations(this IServiceCollection services, Action<IEventStoreConfigurator> configurations) =>
             services.AddSingleton<IEventStoreConfigurator>(provider => 
             {
                 var confs = new EventStoreConfigurations();
                 configurations(confs);
                 return confs;
             });

        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));
         
        private static IServiceCollection RegisterSnapshotableRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(ISnapshotableRootRepository<>), typeof(SnapshotableRootRepository<>));

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, EventStoreUnitOfWork>();

    }

}
