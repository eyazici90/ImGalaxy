using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos
{
    public class CarNameChangedEvent : INotification
    {
        public readonly string Id;
        public readonly string Name;
        public CarNameChangedEvent(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
