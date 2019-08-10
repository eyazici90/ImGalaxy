using ImGalaxy.ES.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Modules
{
    public static class ImGalaxyESCosmoStoreModule
    { 
        public static IServiceCollection AddGalaxyESCosmosDBModule(this IServiceCollection services, Action<ICosmosDBConfigurations> configurations) =>
           services.With(s =>
           {
               s.RegisterConfigurations(configurations)
                .RegisterProviders()
                .RegisterRepositories()
                .RegisterSnapshotableRepositories()
                .RegisterUnitOfWork()
                .RegisterCosmosDbConnection()
                .RegisterCosmosClient();
           });


        private static IServiceCollection RegisterConfigurations(this IServiceCollection services, Action<ICosmosDBConfigurations> configurations) =>
             services.AddSingleton<ICosmosDBConfigurations>(provider => new CosmosDBConfigurations().With(c => configurations(c)));
        private static IServiceCollection RegisterProviders(this IServiceCollection services) =>
             services.AddSingleton<IStreamNameProvider, CosmosStreamNameProvider>()
                     .AddSingleton<IEventSerializer, NewtonsoftJsonSerializer>()
                     .AddSingleton<IEventDeserializer, NewtonsoftJsonSerializer>();

        private static IServiceCollection RegisterRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        private static IServiceCollection RegisterSnapshotableRepositories(this IServiceCollection services) =>
            services.AddScoped(typeof(ISnapshotableRootRepository<>), typeof(SnapshotableRootRepository<>));

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services) =>
             services.AddScoped<IUnitOfWork, CosmosDBUnitOfWork>();

        private static IServiceCollection RegisterCosmosDbConnection(this IServiceCollection services) =>
           services.AddTransient<ICosmosDBConnection, CosmosDBConnection>();

        private static IServiceCollection RegisterCosmosClient(this IServiceCollection services) =>
              services.AddSingleton<IDocumentClient>(ctx => 
              {
                  var confs = ctx.GetRequiredService<ICosmosDBConfigurations>();

                  return new DocumentClient(new Uri(confs.EndpointUri), confs.PrimaryKey);
              })
              .AddSingleton<ICosmosDBClient, CosmosDBClient>();
    }
}
