using ImGalaxy.ES.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestCosmos.Domain.Cars;

namespace TestCosmos.Application.Commands.Handlers
{
    public class AddItemToCarCommandHandler : CommandHandlerBase<CarState, CarId>, IRequestHandler<AddItemToCarCommand>
    {
        public AddItemToCarCommandHandler(IUnitOfWork unitOfWork, 
            IAggregateRootRepository<CarState> rootRepository) 
            : base(unitOfWork, rootRepository)
        {
        }

        public async Task<Unit> Handle(AddItemToCarCommand request, CancellationToken cancellationToken)
        {
            await UpdateAsync(new CarId(request.CarId), async car=> Car.AddCarItem(car, 
                                                                        new CarItemId(Guid.NewGuid().ToString()), request.Desc));
            return Unit.Value;
        }
    }
}
