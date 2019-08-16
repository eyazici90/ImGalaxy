using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Domain.Cars.Snapshots
{
    public class CarStateSnapshot
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int  Year { get; set; }
        public List<CarItemStateSnapshot> CarItems { get; set; }
    }
}
