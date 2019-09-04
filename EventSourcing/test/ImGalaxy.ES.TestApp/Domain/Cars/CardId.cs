using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain.Cars
{ 
    public class CarId
    {
        public static CarId New => new CarId(Guid.NewGuid().ToString());

        public readonly string Id;
        public CarId(string id) => Id = id;
        public override string ToString() => Id;

        public static implicit operator string(CarId self) => self.Id; 

        public static explicit operator CarId(string value) => new CarId(value);
    }
}
