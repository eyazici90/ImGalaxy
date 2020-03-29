using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ImGalaxy.ES.EventStore.Tests.Application.Commands
{
    public class EventStore_When_car_name_changed : When_car_name_changed
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
              ImGalaxyEventStoreTestConfigurator.Configure(services);

    }
}
