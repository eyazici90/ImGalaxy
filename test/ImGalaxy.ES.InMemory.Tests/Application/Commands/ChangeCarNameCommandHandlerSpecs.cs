using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ImGalaxy.ES.InMemory.Tests.Application.Commands
{
    public class InMemory_When_car_name_changed : When_car_name_changed
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
           ImGalaxyInMemoryTestConfigurator.Configure(services);

    }
}
