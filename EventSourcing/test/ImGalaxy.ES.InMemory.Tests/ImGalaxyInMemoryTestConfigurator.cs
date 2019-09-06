﻿using ImGalaxy.ES.InMemory.Modules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Application.Commands.Handlers;

namespace ImGalaxy.ES.InMemory.Tests
{ 
    public static class ImGalaxyInMemoryTestConfigurator
    {
        public static IServiceCollection Configure(IServiceCollection services) =>
             services.AddMediatR(typeof(CreateCarCommandHandler).Assembly)
                     .AddImGalaxyESInMemoryModule();
    }
}