using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain.Cars
{
    public class CarItemId : Identity<string>
    {
        public CarItemId(string id) : base(id)
        {
        }

        public static implicit operator string(CarItemId self) => self.Id;

        public static explicit operator CarItemId(string value) => new CarItemId(value);
    }
}
