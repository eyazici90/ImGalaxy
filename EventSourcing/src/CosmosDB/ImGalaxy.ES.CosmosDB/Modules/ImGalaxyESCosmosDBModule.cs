using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Modules
{
    public static class ImGalaxyESCosmoStoreModule
    {
        public static IServiceCollection AddGalaxyESCosmoStoreModule(this IServiceCollection services, Action<ICosmosDBConfigurator> configurations)
        { 
            return services;
        }
    }
}
