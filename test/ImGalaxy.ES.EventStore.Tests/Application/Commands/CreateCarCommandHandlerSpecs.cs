using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection; 
namespace ImGalaxy.ES.EventStore.Tests.Application.Commands
{
    public class EventStore_When_car_created : When_car_created
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
              ImGalaxyEventStoreTestConfigurator.Configure(services);
    }
}
