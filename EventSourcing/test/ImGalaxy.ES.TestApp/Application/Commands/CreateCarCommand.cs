using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Domain.Cars;

namespace TestApp.Application.Commands
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
