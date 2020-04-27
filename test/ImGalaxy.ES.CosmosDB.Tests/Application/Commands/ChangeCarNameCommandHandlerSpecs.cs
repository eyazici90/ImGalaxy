using ImGalaxy.ES.CosmosDB.Modules;
using ImGalaxy.ES.CosmosDB.Tests;
using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImGalaxy.ES.CosmosDB.Tests.Application.Commands
{
    public class CosmosDb_When_car_name_changed : When_car_name_changed
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
          ImGalaxyCosmosDBTestConfigurator.Configure(services); 
    }
}
