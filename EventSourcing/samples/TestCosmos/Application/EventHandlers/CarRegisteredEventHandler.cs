using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCosmos.Application.EventHandlers
{
    public class CarRegisteredEventHandler : INotificationHandler<CarRegisteredEvent>
    {
        public async Task Handle(CarRegisteredEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Event handled by handler {notification.Id} = {notification.Name}");
        }
    }
}
