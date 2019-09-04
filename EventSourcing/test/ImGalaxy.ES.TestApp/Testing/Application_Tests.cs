using FluentAssertions;
using ImGalaxy.ES.TestBase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Application.Commands;
using Xunit;

namespace ImGalaxy.ES.TestApp.Testing
{
    public abstract class Application_Tests  : ImGalaxyIntegrationTestBase
    {
        private readonly IMediator _mediatr;

        private readonly string _fakeCarId;

        public Application_Tests()
        {
            _mediatr = GetRequiredService<IMediator>();

            _fakeCarId = Guid.NewGuid().ToString();

            SeedCar().ConfigureAwait(false)
                .GetAwaiter().GetResult();
        }

        private async Task SeedCar()
        {
            var fakeName = "BMW";

            var command = new CreateCarCommand(_fakeCarId, fakeName);

            await _mediatr.Send(command);
        }

        [Fact]

        public async Task create_car_command_with_valid_command_should_success()
        {
            var fakeId = Guid.NewGuid().ToString();

            var fakeName = "Mercedes";

            var command = new CreateCarCommand(fakeId, fakeName);

            var result = await _mediatr.Send(command);

            result.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task change_car_model_year_command_with_valid_command_should_success()
        {
            var fakeYear = 2015;

            var command = new ChangeModelYearCommand(_fakeCarId, fakeYear);

            var result = await _mediatr.Send(command);

            result.Should().Be(Unit.Value);

        }

        [Fact]
        public async Task change_car_name_command_with_valid_command_should_success()
        {
            var fakeName = "Ferrari";

            var command = new ChangeCarNameCommand(_fakeCarId, fakeName);

            var result = await _mediatr.Send(command);

            result.Should().Be(Unit.Value);

        }

        [Fact]
        public async Task change_car_model_year_above_2019_command_should_throw()
        {
            var fakeYear = 2020;

            var command = new ChangeModelYearCommand(_fakeCarId, fakeYear);

            Func<Task> act = async () => await _mediatr.Send(command);

            act.Should().Throw<Exception>();
        }
    }
}
