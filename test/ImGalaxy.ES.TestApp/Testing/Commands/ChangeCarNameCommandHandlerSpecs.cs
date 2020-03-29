using FluentAssertions;
using ImGalaxy.ES.TestBase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Application.Commands;
using Xunit;

namespace ImGalaxy.ES.TestApp.Testing.Commands
{
    public abstract class When_car_name_changed : Given_a_car_with_aggregate_store
    {
        private readonly string fakeName = "Ferrari";
        private Unit _result;
        public When_car_name_changed()
        {
            When(async () =>
            {
                var command = new ChangeCarNameCommand(FakeCarId, fakeName);

                _result = await The<IMediator>().Send(command);
            });
        }

        [Fact]
        public void Then_command_should_be_succeed() =>
            _result.Should().Be(Unit.Value);

    }

    public abstract class Given_a_car_with_aggregate_store : GivenWhenThen
    {
        public string FakeCarId { get; }
        public IMediator Mediatr { get; }
        public Given_a_car_with_aggregate_store()
        {
            Mediatr = The<IMediator>();

            FakeCarId = Guid.NewGuid().ToString();

            Given(async () => await SeedCar());

        }

        private async Task SeedCar()
        {
            var fakeName = "BMW";

            var command = new CreateCarCommand(FakeCarId, fakeName);

            await Mediatr.Send(command);
        }
    }
}
