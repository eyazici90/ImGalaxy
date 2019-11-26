using System;
using System.Collections.Generic;
using System.Text;
using TestApp;

namespace ImGalaxy.ES.TestApp.Domain.Cars
{
    public class CannotBeAddedMoreThanTwoItems
    {
        public CarState Car { get; }
        public CannotBeAddedMoreThanTwoItems(CarState car)
        {
            Car = car;
        }
    }
}
