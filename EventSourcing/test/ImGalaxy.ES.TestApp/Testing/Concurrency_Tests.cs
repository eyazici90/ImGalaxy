using FluentAssertions;
using ImGalaxy.ES.Core;
using ImGalaxy.ES.TestBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp;
using TestApp.Domain.Cars;
using Xunit;

namespace ImGalaxy.ES.TestApp.Testing
{
    public abstract class Concurrency_Tests : ImGalaxyIntegrationTestBase
    {
        private readonly string _fakeCarId;

        private readonly IAggregateRootRepository<CarState> _aggregateRootRepository;
        public Concurrency_Tests()
        {
            _aggregateRootRepository = GetRequiredService<IAggregateRootRepository<CarState>>();

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
        public async Task should_not_allow_to_update_if_the_entity_has_changed()
        {
            var existingCar1 = await _aggregateRootRepository.GetAsync(_fakeCarId); 

            var existingCar2 = await _aggregateRootRepository.GetAsync(_fakeCarId); 

            var result = await WithUnitOfWorkAsync(async () =>
            {
                Car.ChangeName(existingCar2.Value, "Changed-2");

            });

            result.IsSuccess.Should().BeTrue();

            Func<Task> func = async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    Car.ChangeName(existingCar1.Value, "Changed-1");
                });
            };

            func.Should().Throw<Exception>();


        }
    }
}
