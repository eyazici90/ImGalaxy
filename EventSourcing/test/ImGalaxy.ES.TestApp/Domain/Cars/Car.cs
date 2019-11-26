using ImGalaxy.ES.Core;
using ImGalaxy.ES.TestApp.Domain.Cars;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain.Cars
{
    public static class Car
    {
        public static CarState.Result RegisterCar(string id, string name) =>
         new CarState(new CarId(id)).ApplyEvent(new CarRegisteredEvent(id, name));

        public static CarState.Result ChangeName(CarState state, string name) =>
            state.ApplyEvent(new CarNameChangedEvent(state.Id, name));

        public static CarState.Result RenewModel(CarState state, int year, ICarPolicy carPolicy) =>
            state.With(s => carPolicy.Apply(new ModelYearCannotBeAboveThan(year)))
                .ApplyEvent(new CarModelRenewedEvent(state.Id, year));

        public static CarState.Result AddCarItem(CarState state, CarItemId carItemId, string desc, ICarPolicy carPolicy) =>
             state.With(s => carPolicy.Apply(new CannotBeAddedMoreThanTwoItems(s)))
                 .ApplyEvent(new CarItemAddedEvent(state.Id, carItemId, desc));
    }
}
