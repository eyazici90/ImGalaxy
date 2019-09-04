using ImGalaxy.ES.TestApp.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore.Tests.Repositories
{
    public class EventStore_Repository_Tests : Repository_Tests
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
            ImGalaxyEventStoreTestConfigurator.Configure(services);
    }
}
