﻿using ImGalaxy.ES.TestApp.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore.Tests.Application
{
    public class EventStore_Application_Tests : Application_Tests
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services)=>
             ImGalaxyEventStoreTestConfigurator.Configure(services);
    }
}
