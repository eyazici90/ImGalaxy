using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory.Tests.Application.Commands
{
    public class InMemory_When_car_created : When_car_created
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
              ImGalaxyInMemoryTestConfigurator.Configure(services);
    }
}
