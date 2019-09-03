using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Application.Commands
{
    public class AddItemToCarCommand : IRequest
    {
        public readonly string CarId;
        public readonly string Desc;
        public AddItemToCarCommand(string carId, string desc)
        {
            CarId = carId;
            Desc = desc;
        }
    }
}
