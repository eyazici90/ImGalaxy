using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB;
using ImGalaxy.ES.CosmosDB.Internal;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImGalaxyESCosmosDBModule(this IServiceCollection services, Action<ICosmosDBConfigurations> configurations) =>
           services.With(s =>
           {
               var configs = new CosmosDBConfigurations().With(c => configurations(c));

               s.RegisterConfigurations(configs)
                .RegisterProviders()
                .RegisterChangeTracker()
                .RegisterAggregateStore()
                .RegisterOperations()
                .RegisterRepositories()
                .RegisterSnapshotableRepositories(configs)
                .RegisterUnitOfWork()
                .RegisterCosmosDbConnection()
                .RegisterCosmosClient();

           });


        private static IServiceCollection RegisterConfigurations(this IServiceCollection services, CosmosDBConfigurations configurations) =>
             services.AddSingleton<ICosmosDBConfigurations>(provider => configurations);
        private static IServiceCollection RegisterProviders(this IServiceCollection services) =>
             services.AddSingleton<IStreamNameProvider, CosmosStreamNameProvider>()
                     .AddSingleton<IEventSerializer, NewtonsoftJsonSerializer>()
                     .AddSingleton<IEventDeserializer, NewtonsoftJsonSerializer>()
                     .AddTransient<IAggregateStoreDependencies, AggregateStoreDependencies>()
                     .AddTransient<ISnapshotReader, SnapshotReaderCosmosDB>();
        private static IServiceCollection RegisterChangeTracker(this IServiceCollection services) =>
            services.AddScoped<IAggregateChangeTracker, AggregateChangeTracker>();

        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        private static IServiceCollection RegisterSnapshotableRepositories(this IServiceCollection services, ICosmosDBConfigurations configurations) =>
            configurations.IsSnapshottingOn ? services.AddScoped(typeof(IAggregateRootRepository<>), typeof(SnapshotableRootRepository<>))
                                            : services;

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, CosmosDBUnitOfWork>();

        private static IServiceCollection RegisterCosmosDbConnection(this IServiceCollection services) =>
           services.AddSingleton<ICosmosDBConnection, CosmosDBConnection>();

        private static IServiceCollection RegisterCosmosClient(this IServiceCollection services) =>
              services.AddSingleton(ctx =>
              {
                  var confs = ctx.GetRequiredService<ICosmosDBConfigurations>();

                  return new CosmosClient(confs.EndpointUri, confs.PrimaryKey);
              })
              .AddSingleton<ICosmosDBClient, CosmosDBClient>();

        private static IServiceCollection RegisterAggregateStore(this IServiceCollection services) =>
              services.AddTransient<IAggregateStore, AggregateStore>();

        private static IServiceCollection RegisterOperations(this IServiceCollection services) =>
              services.AddSingleton<IOperationDispatcher, OperationDispatcher>();
         

        [Obsolete]
        public static async Task<IServiceProvider> UseGalaxyESCosmosDBModule(this IServiceProvider provider,
            string partitionKeyPath)
        {
            var confs = provider.GetRequiredService<ICosmosDBConfigurations>();

            var cosmosClient = provider.GetRequiredService<ICosmosDBClient>();

            await cosmosClient.CreateDatabaseIfNotExistsAsync();

            await cosmosClient.CreateContainerIfNotExistsAsync(confs.StreamContainerName, partitionKeyPath);

            await cosmosClient.CreateContainerIfNotExistsAsync(confs.EventContainerName, partitionKeyPath);

            if (confs.IsSnapshottingOn)
                await cosmosClient.CreateContainerIfNotExistsAsync(confs.SnapshotContainerName, partitionKeyPath);

            return provider;
        }

    }
}
