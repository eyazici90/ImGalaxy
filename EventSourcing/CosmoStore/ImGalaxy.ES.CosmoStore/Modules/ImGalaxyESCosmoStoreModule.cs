using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmoStore.Modules
{
    public static class ImGalaxyESCosmoStoreModule
    {
        public static IServiceCollection AddCosmoStoreModule(this IServiceCollection services, Action<ICosmoConfigurator> configurations)
        { 
            return services;
        }
    }
}
