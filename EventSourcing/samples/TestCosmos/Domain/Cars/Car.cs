using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Domain.Cars
{
    public static class Car
    {
        public static CarState.Result RegisterCar(string id, string name) =>
         new CarState(new CarId(id)).ApplyEvent(new CarRegisteredEvent(id, name));

        public static CarState.Result ChangeName(CarState state, string name) =>
            state.ApplyEvent(new CarNameChangedEvent(state._id, name));

        public static CarState.Result RenewModel(CarState state, int year) =>
            state.ThrowsIf(c => c._year > year, new Exception("model cannot be above than 2019"))
                .ApplyEvent(new CarModelRenewedEvent(state._id, year));

        public static CarState.Result AddCarItem(CarState state, CarItemId carItemId, string desc)=>
            state.ApplyEvent(new CarItemAddedEvent(state._id, carItemId, desc));
    }
}
