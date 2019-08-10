using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using ImGalaxy.ES.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore.Modules
{
    public static class ImGalaxyESEventStoreModule
    {
        public static IServiceCollection AddGalaxyESEventStoreModule(this IServiceCollection services, Action<IEventStoreConfigurations> configurations) =>
            services.With(s=> 
            {
                s.RegisterConfigurations(configurations)
                 .RegisterProviders()
                 .RegisterEventStore(configurations)
                 .RegisterRepositories()
                 .RegisterSnapshotableRepositories()
                 .RegisterUnitOfWork();
            });
     

        private static IServiceCollection RegisterConfigurations(this IServiceCollection services, Action<IEventStoreConfigurations> configurations) =>
             services.AddSingleton<IEventStoreConfigurations>(provider => new EventStoreConfigurations().With(c=> configurations(c)));
        private static IServiceCollection RegisterProviders(this IServiceCollection services) =>
             services.AddSingleton<IStreamNameProvider, EventStoreStreamNameProvider>()
                     .AddSingleton<IEventSerializer, NewtonsoftJsonSerializer>()
                     .AddSingleton<IEventDeserializer, NewtonsoftJsonSerializer>();
        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
             services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));
          
        private static IServiceCollection RegisterSnapshotableRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(ISnapshotableRootRepository<>), typeof(SnapshotableRootRepository<>));

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, EventStoreUnitOfWork>();

        private static IServiceCollection RegisterEventStore(this IServiceCollection services, Action<IEventStoreConfigurations> configurationsAction)
        {
            var configs = new EventStoreConfigurations();
            configurationsAction(configs);

            ConnectionSettings settings = ConnectionSettings.Create()
                                                            .SetDefaultUserCredentials(new UserCredentials(configs.Username, configs.Password)).Build();

            //builder.Register(c => {
            //    IEventStoreConnection connection = EventStoreConnection.Create(settings, new Uri(configs.Uri));
            //    connection.ConnectAsync().ConfigureAwait(false)
            //   .GetAwaiter()
            //   .GetResult();
            //    return connection;
            //})
            //.As<IEventStoreConnection>()
            //.SingleInstance();

            return services;
        }
    }

}
