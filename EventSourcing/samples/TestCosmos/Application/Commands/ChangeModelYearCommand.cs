using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Application.Commands
{
    public class ChangeModelYearCommand : IRequest
    {
        public readonly string CarId;
        public readonly int Year;
        public ChangeModelYearCommand(string carId, int year)
        {
            CarId = carId;
            Year = year;
        }
    }
}
