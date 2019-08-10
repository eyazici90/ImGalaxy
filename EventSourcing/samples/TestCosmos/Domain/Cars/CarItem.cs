using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Domain.Cars
{
    public static class CarItem
    {
        public static CarItemState.Result Create(string id, CarId carId, string desc) =>
            new CarItemState(new CarItemId(id), carId)
                .ApplyEvent(new CarItemAddedEvent(carId, new CarItemId(id), desc));
    }
}
