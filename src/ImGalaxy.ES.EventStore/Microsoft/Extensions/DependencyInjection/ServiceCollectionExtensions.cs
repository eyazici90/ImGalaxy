using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using ImGalaxy.ES.Core;
using ImGalaxy.ES.EventStore; 
using System; 

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImGalaxyESEventStoreModule(this IServiceCollection services, Action<IEventStoreConfigurations> configurations) =>
            services.With(s =>
            {
                var configs = new EventStoreConfigurations().With(c => configurations(c));
                s.RegisterConfigurations(configs)
                 .RegisterProviders()
                 .RegisterChangeTracker()
                 .RegisterEventStore()
                 .RegisterAggregateStore()
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
                     .AddTransient<IAggregateStoreDependencies, AggregateStoreDependencies>()
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
            services.AddSingleton(ctx =>
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

        private static IServiceCollection RegisterAggregateStore(this IServiceCollection services) =>
              services.AddTransient<IAggregateStore, AggregateStore>();

    }

}
