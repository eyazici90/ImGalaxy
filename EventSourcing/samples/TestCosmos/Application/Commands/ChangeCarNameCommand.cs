using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCosmos.Application.Commands
{
    public class ChangeCarNameCommand : IRequest
    {
        public readonly string CarId;
        public readonly string Name;
        public ChangeCarNameCommand(string carId, string name)
        {

            CarId = carId;
            Name = name;
        }
    }
}
