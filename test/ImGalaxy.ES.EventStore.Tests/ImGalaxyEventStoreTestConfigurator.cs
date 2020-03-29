using ImGalaxy.ES.EventStore.Modules;
using ImGalaxy.ES.TestApp.Domain.Cars;
using ImGalaxy.ES.TestApp.Infrastructure.Cars;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Application.Commands.Handlers;

namespace ImGalaxy.ES.EventStore.Tests
{
    public static class ImGalaxyEventStoreTestConfigurator
    {
        public static IServiceCollection Configure(IServiceCollection services) =>
           services.AddMediatR(typeof(CreateCarCommandHandler).Assembly)
                   .AddTransient<ICarPolicy, CarPolicy>()
                   .AddImGalaxyESEventStoreModule(configs =>
                   {
                       configs.Uri = "tcp://admin:changeit@localhost:1113";
                       configs.Username = "admin";
                       configs.Password = "changeit";
                       configs.ReadBatchSize =   1000;
                       configs.MaxLiveQueueSize = 100;
                       configs.IsSnapshottingOn = true;
                   });
    }
}
