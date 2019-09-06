using ImGalaxy.ES.TestApp.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory.Tests.Repositories
{
    public class InMemory_Repository_Tests : Repository_Tests
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
                ImGalaxyInMemoryTestConfigurator.Configure(services);
    }
}
