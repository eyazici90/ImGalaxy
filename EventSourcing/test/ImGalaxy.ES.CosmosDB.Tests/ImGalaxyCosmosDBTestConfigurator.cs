﻿using ImGalaxy.ES.CosmosDB.Modules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Application.Commands.Handlers;

namespace ImGalaxy.ES.CosmosDB.Tests
{
    public static class ImGalaxyCosmosDBTestConfigurator
    {
        public static IServiceCollection Configure(IServiceCollection services) =>
             services.AddMediatR(typeof(CreateCarCommandHandler).Assembly)
                     .AddImGalaxyESCosmosDBModule(configs =>
                     {
                        configs.DatabaseId = $"TestApp";
                        configs.EventCollectionName = "Events";
                        configs.StreamCollectionName = "Streams";
                        configs.SnapshotCollectionName = "Snapshots";
                        configs.EndpointUri = "https://localhost:8081";
                        configs.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                        configs.ReadBatchSize = 1000;
                        configs.IsSnapshottingOn = true;
                        configs.OfferThroughput = 10000;
                     });
    }
}