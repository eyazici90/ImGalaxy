using FluentAssertions;
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
    public abstract class Domain_Tests
    {
        [Fact]
        public void creating_new_car_with_valid_state_should_success() =>
            CommandScenarioFor<CarState>.With(
                   FakeCar.State
                   )
                .GivenNone()
                .WhenNone()
                .ThenNone()
                .Assert(s => s.Should().NotBeNull());

        [Fact]
        public void change_existing_car_name_should_success()
        {
            var fakeCar = FakeCar;
            var fakeName = "Mercedes";
            CommandScenarioFor<CarState>.With(
                   fakeCar.State
                   )
                .GivenNone()
                .When(state => Car.ChangeName(state, fakeName))
                .Then(new CarNameChangedEvent(fakeCar.State.Id, fakeName))
                .Assert();
        }

        [Fact]
        public void renew_existing_car_model_should_success()
        {
            var fakeCar = FakeCar;
            var fakeYear = 2014;
            CommandScenarioFor<CarState>.With(
                   fakeCar.State
                   )
                .GivenNone()
                .When(state => Car.RenewModel(state, fakeYear))
                .Then(new CarModelRenewedEvent(fakeCar.State.Id, fakeYear))
                .Assert();
        }

        [Fact]
        public void renew_existing_car_model_above_2019_should_throw()
        {
            var fakeCar = FakeCar;
            var fakeYear = 2020;
            CommandScenarioFor<CarState>.With(
                   fakeCar.State
                   )
                .GivenNone()
                .When(state => Car.RenewModel(state, fakeYear))
                .Throws(new Exception("model cannot be above than 2019"))
                .Assert();
        }
         
        public CarState.Result FakeCar
        {
            get
            {
                var newCar = Car.RegisterCar(Guid.NewGuid().ToString(), "Fake-1");
                newCar.State.ClearEvents();
                return newCar;
            }
        }
    }
}
