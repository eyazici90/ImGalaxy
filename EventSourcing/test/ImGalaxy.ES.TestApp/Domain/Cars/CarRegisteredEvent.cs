using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public class CarRegisteredEvent : INotification
    {
        public readonly string Id;
        public readonly string Name;
        public CarRegisteredEvent(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
