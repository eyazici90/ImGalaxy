using ImGalaxy.ES.TestApp.Domain.Cars;
using ImGalaxy.ES.TestApp.Infrastructure.Cars;
using MediatR;
using Microsoft.Extensions.DependencyInjection; 
using TestApp.Application.Commands.Handlers;

namespace ImGalaxy.ES.InMemory.Tests
{ 
    public static class ImGalaxyInMemoryTestConfigurator
    {
        public static IServiceCollection Configure(IServiceCollection services) =>
             services.AddMediatR(typeof(CreateCarCommandHandler).Assembly)
                     .AddTransient<ICarPolicy, CarPolicy>()
                     .AddImGalaxyESInMemoryModule();
    }
}
