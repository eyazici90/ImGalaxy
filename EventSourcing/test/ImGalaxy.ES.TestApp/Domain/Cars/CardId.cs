using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain.Cars
{ 
    public class CarId : Identity<string>
    {
        public CarId(string id) 
            : base(id)
        {
        }
        public static CarId New => new CarId(Guid.NewGuid().ToString());

        public static implicit operator string(CarId self) => self.Id; 

        public static explicit operator CarId(string value) => new CarId(value);
    }
}
