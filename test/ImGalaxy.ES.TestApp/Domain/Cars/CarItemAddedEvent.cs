using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain.Cars
{
    public class CarItemAddedEvent : INotification
    {
        public readonly CarId CarId;
        public readonly CarItemId CarItemId;
        public readonly string Desciption;
        public CarItemAddedEvent(CarId carId, CarItemId carItemId, string desciption)
        {
            CarId = carId;
            CarItemId = carItemId;
            Desciption = desciption;
        }
    }
}
