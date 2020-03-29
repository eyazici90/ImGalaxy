using FluentAssertions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Application.Commands;
using Xunit;

namespace ImGalaxy.ES.TestApp.Testing.Commands
{
    public abstract class When_car_model_year_changed : Given_a_car_with_aggregate_store
    {
        private readonly int fakeYear = 2015;
        private Unit _result;
        public When_car_model_year_changed()
        {
            When(async () =>
            {
                var command = new ChangeModelYearCommand(FakeCarId, fakeYear);

                _result = await The<IMediator>().Send(command);
            });

        }

        [Fact]
        public void Then_command_should_be_succeed() =>
          _result.Should().Be(Unit.Value);
    }

    public abstract class When_car_model_year_changed_above_2019 : Given_a_car_with_aggregate_store
    {
        private readonly int fakeYear = 2020;
        private Func<Task> _act;
        public When_car_model_year_changed_above_2019()
        {
            When(async () =>
            {
                var command = new ChangeModelYearCommand(FakeCarId, fakeYear);

                _act = async () => await The<IMediator>().Send(command);
            });

        }

        [Fact]
        public void Then_should_throw() =>
          _act.Should().Throw<Exception>();
    }

}
