using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;
namespace ImGalaxy.ES.InMemory.Tests.Application.Commands
{

    public class InMemory_When_car_model_year_changed : When_car_model_year_changed
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
           ImGalaxyInMemoryTestConfigurator.Configure(services);

    }
    public class InMemory_When_car_model_year_changed_above_2019 : When_car_model_year_changed_above_2019
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
           ImGalaxyInMemoryTestConfigurator.Configure(services);

    }

}
