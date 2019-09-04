using ImGalaxy.ES.CosmosDB.Modules;
using ImGalaxy.ES.TestApp.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Tests.Application
{
    public class Cosmos_Application_Tests : Application_Tests
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
            ImGalaxyCosmosDBTestConfigurator.Configure(services);

        protected override void Configure(IServiceProvider app)=>
            app.UseGalaxyESCosmosDBModule().ConfigureAwait(false)
                .GetAwaiter().GetResult();
        
    }
}
