﻿using FluentAssertions;
using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Modules;
using ImGalaxy.ES.TestBase;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp;
using TestApp.Domain.Cars;
using Xunit;

namespace ImGalaxy.ES.CosmosDB.Tests.Connection
{
    public class CosmosDbConnection_Tests : ImGalaxyIntegrationTestBase
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
          ImGalaxyCosmosDBTestConfigurator.Configure(services);

        protected override void Configure(IServiceProvider app) =>
            app.UseGalaxyESCosmosDBModule().ConfigureAwait(false)
                .GetAwaiter().GetResult();


        private readonly ICosmosDBConnection _cosmosDBConnection;
        private readonly IStreamNameProvider _streamNameProvider;
        private readonly IAggregateRootRepository<CarState> _aggregateRootRepository;
        private readonly string _fakeCarId;
        public CosmosDbConnection_Tests()
        {
            _cosmosDBConnection = GetRequiredService<ICosmosDBConnection>();
            _aggregateRootRepository = GetRequiredService<IAggregateRootRepository<CarState>>();
            _streamNameProvider = GetRequiredService<IStreamNameProvider>();
            _fakeCarId = Guid.NewGuid().ToString();

            SeedCar().ConfigureAwait(false)
                .GetAwaiter().GetResult();
        }

        private async Task SeedCar()
        {
            var fakeName = "Bugatti";

            var fakeCar = Car.RegisterCar(_fakeCarId, fakeName);

            await WithUnitOfWorkAsync(async () =>
            {
                await _aggregateRootRepository.AddAsync(fakeCar.State, _fakeCarId);
            });
        }

        [Fact]
        public async Task concurrent_append_to_stream_events_should_be_ordered()
        {
            var existingCar = await _aggregateRootRepository.GetAsync(_fakeCarId);


            var taskList = new Task[100];

            for (int i = 0; i < 100; i++)
            {
                taskList[i] = Task.Run(() =>
                {
                    try
                    {
                        WithUnitOfWorkAsync(async () =>
                        {
                            Car.ChangeName(existingCar.Value, $"Fake-{i.ToString()}");
                        }).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch 
                    {
                    } 
                });
            }
             
            await Task.WhenAll(taskList);

            var streamId = _streamNameProvider.GetStreamName(typeof(CarState), _fakeCarId);
            
            var streamEvents = await _cosmosDBConnection.ReadStreamEventsForwardAsync(streamId, 1, 1000);

            var eventCount = streamEvents.Value.Events.Length;

            var distinctedCount = streamEvents.Value.Events.ToList().Select(e => e.Position)
                .Distinct()
                .Count();

            eventCount.Should().Be(distinctedCount);
        }

    }
}