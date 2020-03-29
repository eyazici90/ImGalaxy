using FluentAssertions;
using ImGalaxy.ES.Core;
using ImGalaxy.ES.TestApp.Domain.Cars;
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
    public abstract class Repository_Tests : ImGalaxyIntegratedTestBase
    {
        private readonly string _fakeCarId;

        private readonly IAggregateRootRepository<CarState> _aggregateRootRepository;

        private readonly ICarPolicy _carPolicy;
        public Repository_Tests()
        {
            _aggregateRootRepository = GetRequiredService<IAggregateRootRepository<CarState>>();

            _fakeCarId = Guid.NewGuid().ToString();

            _carPolicy = GetRequiredService<ICarPolicy>();

            SeedCar().ConfigureAwait(false)
                .GetAwaiter().GetResult();
        }

        private async Task SeedCar()
        {
            var fakeName = "Audi";

            var fakeCar = Car.RegisterCar(_fakeCarId, fakeName);

            await WithUnitOfWorkAsync(async () =>
            {
                await _aggregateRootRepository.AddAsync(fakeCar.State, _fakeCarId);
            });

        }

        [Fact]
        public async Task create_should_success()
        {
            var newId = Guid.NewGuid().ToString();

            var fakeCar = Car.RegisterCar(newId, "Fake-1");

            var result = await WithUnitOfWorkAsync(async () =>
            {
                await _aggregateRootRepository.AddAsync(fakeCar.State, newId);
            });

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task get_existing_record_from_repository_shuold_return_record()
        {
            var existingCar = await _aggregateRootRepository.GetAsync(_fakeCarId);

            existingCar.Should().NotBeNull();
        }

        [Fact]
        public async Task update_existing_record_should_update_record_success()
        {
            var existingCar = await _aggregateRootRepository.GetAsync(_fakeCarId);

            var fakeYear = 2013;
            
            var result = await WithUnitOfWorkAsync(async () =>
            {
                Car.RenewModel(existingCar.Value, fakeYear, _carPolicy);
            });

            result.IsSuccess.Should().BeTrue();

            var updatedCar = await _aggregateRootRepository.GetAsync(_fakeCarId);

            updatedCar.Value.Year.Should().Be(fakeYear);
        }
    }
}
