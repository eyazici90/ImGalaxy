using FluentAssertions;
using ImGalaxy.ES.TestBase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Application.Commands;
using Xunit;

namespace ImGalaxy.ES.TestApp.Testing.Commands
{
    public abstract class When_car_created : GivenWhenThen
    {
        private readonly string _fakeId = Guid.NewGuid().ToString();
        private readonly string _fakeName = "Mercedes";
        private Unit _result;
        public When_car_created()
        {
            When(async () =>
            {
                var command = new CreateCarCommand(_fakeId, _fakeName);

                _result = await The<IMediator>().Send(command);
            });

        }

        [Fact]
        public void Then_command_should_be_succeed() =>
          _result.Should().Be(Unit.Value);
    }
}
