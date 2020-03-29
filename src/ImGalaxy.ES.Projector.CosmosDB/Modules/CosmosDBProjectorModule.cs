using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Projector.CosmosDB.Modules
{
    public static class CosmosDBProjectorModule
    {
        public static IServiceCollection UseProjectorForCosmosDb(this IServiceCollection services,
             Action<CosmosOptions> configure,
             Action projectionMappings)
        {
            var setting = new CosmosOptions();

            configure(setting);

            services.AddSingleton<IDocumentClient>(s =>
              new DocumentClient(serviceEndpoint: new Uri(setting.EndpointUri),
              authKeyOrResourceToken: setting.PrimaryKey,
              serializerSettings: new JsonSerializerSettings
              {
                  DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                  ContractResolver = new CustomMapContractResolver()
              }));

            services.AddSingleton<IProjector, CosmosProjector>();

            projectionMappings();

            services.AddSingleton(s => setting);

            return services;
        }

    }
}
