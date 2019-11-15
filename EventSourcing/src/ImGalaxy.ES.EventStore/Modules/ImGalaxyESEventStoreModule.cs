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
        public static IServiceCollection AddImGalaxyESEventStoreModule(this IServiceCollection services, Action<IEventStoreConfigurations> configurations) =>
            services.With(s =>
            {
                var configs = new EventStoreConfigurations().With(c => configurations(c));
                s.RegisterConfigurations(configs)
                 .RegisterProviders()
                 .RegisterChangeTracker()
                 .RegisterEventStore()
                 .RegisterRepositories()
                 .RegisterSnapshotableRepositories(configs)
                 .RegisterUnitOfWork();
            });


        private static IServiceCollection RegisterConfigurations(this IServiceCollection services, EventStoreConfigurations configurations) =>
             services.AddSingleton<IEventStoreConfigurations>(provider => configurations);
        private static IServiceCollection RegisterProviders(this IServiceCollection services) =>
             services.AddSingleton<IStreamNameProvider, EventStoreStreamNameProvider>()
                     .AddSingleton<IEventSerializer, NewtonsoftJsonSerializer>()
                     .AddSingleton<IEventDeserializer, NewtonsoftJsonSerializer>()
                     .AddTransient<IAggregateRootRepositoryBaseDependencies, AggregateRootRepositoryBaseDependencies>()
                     .AddTransient<ISnapshotReader, SnapshotReaderEventStore>();
        private static IServiceCollection RegisterChangeTracker(this IServiceCollection services) =>
           services.AddScoped<IChangeTracker, ChangeTracker>();

        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
             services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        private static IServiceCollection RegisterSnapshotableRepositories(this IServiceCollection services, IEventStoreConfigurations configurations) =>
         configurations.IsSnapshottingOn ? services.AddScoped(typeof(IAggregateRootRepository<>), typeof(SnapshotableRootRepository<>))
                                         : services;

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, EventStoreUnitOfWork>();

        private static IServiceCollection RegisterEventStore(this IServiceCollection services) =>
            services.AddSingleton<IEventStoreConnection>(ctx =>
            {
                var confs = ctx.GetRequiredService<IEventStoreConfigurations>();

                ConnectionSettings settings = ConnectionSettings.Create()
                                                                .SetDefaultUserCredentials(new UserCredentials(confs.Username, confs.Password)).Build();

                IEventStoreConnection connection = EventStoreConnection.Create(settings, new Uri(confs.Uri));
                connection.ConnectAsync().ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

                return connection;
            });

    }

}
