using ImGalaxy.ES.TestApp.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory.Tests.Application
{ 
    public class InMemory_Application_Tests : Application_Tests
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
            ImGalaxyInMemoryTestConfigurator.Configure(services);
         

    }
}
