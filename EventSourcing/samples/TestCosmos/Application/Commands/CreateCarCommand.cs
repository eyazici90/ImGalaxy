using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TestCosmos.Domain.Cars;

namespace TestCosmos.Application.Commands
{
    public class CreateCarCommand: IRequest
    { 
        public readonly string Name;
        public CreateCarCommand(string name)
        {
            Name = name;
        }
    }
}
