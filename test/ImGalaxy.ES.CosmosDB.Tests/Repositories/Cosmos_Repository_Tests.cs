using ImGalaxy.ES.TestApp.Testing;
using Microsoft.Extensions.DependencyInjection; 

namespace ImGalaxy.ES.CosmosDB.Tests.Repositories
{
    public class Cosmos_Repository_Tests : Repository_Tests
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
              ImGalaxyCosmosDBTestConfigurator.Configure(services); 
    }
}
