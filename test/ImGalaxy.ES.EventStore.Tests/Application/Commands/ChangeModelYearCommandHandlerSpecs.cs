using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ImGalaxy.ES.EventStore.Tests.Application.Commands
{

    public class EventStore_When_car_model_year_changed : When_car_model_year_changed
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
           ImGalaxyEventStoreTestConfigurator.Configure(services);

    }
    public class EventStore_When_car_model_year_changed_above_2019 : When_car_model_year_changed_above_2019
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
           ImGalaxyEventStoreTestConfigurator.Configure(services);

    }

}
