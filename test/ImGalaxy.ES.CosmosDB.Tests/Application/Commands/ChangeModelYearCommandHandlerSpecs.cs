﻿using ImGalaxy.ES.CosmosDB.Modules;
using ImGalaxy.ES.CosmosDB.Tests;
using ImGalaxy.ES.TestApp.Testing.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImGalaxy.ES.CosmosDB.Tests.Application.Commands
{

    public class CosmosDb_When_car_model_year_changed : When_car_model_year_changed
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
             ImGalaxyCosmosDBTestConfigurator.Configure(services);

        protected override void Configure(IServiceProvider app) =>
            app.UseGalaxyESCosmosDBModule().ConfigureAwait(false)
                .GetAwaiter().GetResult();
    }
    public class CosmosDb_When_car_model_year_changed_above_2019 : When_car_model_year_changed_above_2019
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
         ImGalaxyCosmosDBTestConfigurator.Configure(services);

        protected override void Configure(IServiceProvider app) =>
            app.UseGalaxyESCosmosDBModule().ConfigureAwait(false)
                .GetAwaiter().GetResult();

    }

}
