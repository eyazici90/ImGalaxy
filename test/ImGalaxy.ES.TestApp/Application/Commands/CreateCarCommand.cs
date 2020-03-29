using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Domain.Cars;

namespace TestApp.Application.Commands
{
    public class CreateCarCommand : IRequest
    {
        public readonly string Id;
        public readonly string Name;
        public CreateCarCommand(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
