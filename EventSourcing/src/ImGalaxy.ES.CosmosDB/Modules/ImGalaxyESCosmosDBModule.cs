using ImGalaxy.ES.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Modules
{
    public static class ImGalaxyESCosmoStoreModule
    { 
        public static IServiceCollection AddImGalaxyESCosmosDBModule(this IServiceCollection services, Action<ICosmosDBConfigurations> configurations) =>
           services.With(s =>
           {
               var configs = new CosmosDBConfigurations().With(c => configurations(c));

               s.RegisterConfigurations(configs)
                .RegisterProviders()
                .RegisterChangeTracker()
                .RegisterAggregateStore()
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
            services.AddScoped<IChangeTracker, ChangeTracker>();

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
              services.AddSingleton<IDocumentClient>(ctx => 
              {
                  var confs = ctx.GetRequiredService<ICosmosDBConfigurations>();

                  return new DocumentClient(new Uri(confs.EndpointUri), confs.PrimaryKey);
              })
              .AddSingleton<ICosmosDBClient, CosmosDBClient>();

        private static IServiceCollection RegisterAggregateStore(this IServiceCollection services) =>
              services.AddTransient<IAggregateStore, AggregateStore>();

        public static async Task<IServiceProvider> UseGalaxyESCosmosDBModule(this IServiceProvider provider)
        {
            var confs = provider.GetRequiredService<ICosmosDBConfigurations>();

            var cosmosClient = provider.GetRequiredService<ICosmosDBClient>();

            await cosmosClient.CreateDatabaseIfNotExistsAsync();

            await cosmosClient.CreateCollectionIfNotExistsAsync(confs.StreamCollectionName);

            await cosmosClient.CreateCollectionIfNotExistsAsync(confs.EventCollectionName);

            if (confs.IsSnapshottingOn)
                await cosmosClient.CreateCollectionIfNotExistsAsync(confs.SnapshotCollectionName);

            return provider;
        }
        
    }
}
