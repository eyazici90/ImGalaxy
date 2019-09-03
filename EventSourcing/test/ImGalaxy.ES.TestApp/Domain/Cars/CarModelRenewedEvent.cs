using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public class CarModelRenewedEvent : INotification
    {
        public readonly string Id;
        public readonly int Year;
        public CarModelRenewedEvent(string id, int year)
        {
            Id = id;
            Year = year;
        }
    }
}
