using ImGalaxy.ES.CosmosDB.Modules;
using ImGalaxy.ES.TestApp.Domain.Cars;
using ImGalaxy.ES.TestApp.Infrastructure.Cars;
using MediatR;
using Microsoft.Extensions.DependencyInjection; 
using TestApp.Application.Commands.Handlers;

namespace ImGalaxy.ES.CosmosDB.Tests
{
    public static class ImGalaxyCosmosDBTestConfigurator
    {
        public static IServiceCollection Configure(IServiceCollection services) =>
             services.AddMediatR(typeof(CreateCarCommandHandler).Assembly) 
                    .AddTransient<ICarPolicy, CarPolicy>()
                    .AddImGalaxyESCosmosDBModule(configs =>
                     {
                         configs.DatabaseId = $"TestApp";
                         configs.EventCollectionName = "Events";
                         configs.StreamCollectionName = "Streams";
                         configs.SnapshotCollectionName = "Snapshots";
                         configs.EndpointUri = ".";
                         configs.PrimaryKey = ".";
                         configs.ReadBatchSize = 1000;
                         configs.IsSnapshottingOn = true;
                         configs.OfferThroughput = 400;
                     });
    }
}
