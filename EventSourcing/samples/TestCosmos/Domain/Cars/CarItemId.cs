using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Domain.Cars
{
    public class CarItemId
    {
        public readonly string Id;
        public CarItemId(string id) => Id = id;
        public override string ToString() => Id;

        public static implicit operator string(CarItemId self) => self.Id;

        public static explicit operator CarItemId(string value) => new CarItemId(value);
    }
}
