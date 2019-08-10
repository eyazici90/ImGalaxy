using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Domain.Cars
{ 
    public class CarId
    {
        public readonly string Id;
        public CarId(string id) => Id = id;
        public override string ToString() => Id;

        public static implicit operator string(CarId self) => self.Id; 

        public static explicit operator CarId(string value) => new CarId(value);
    }
}
