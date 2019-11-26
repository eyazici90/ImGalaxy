using ImGalaxy.ES.Core;
using ImGalaxy.ES.TestApp.Domain.Cars;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Domain.Cars;

namespace TestApp.Application.Commands.Handlers
{
    public class AddItemToCarCommandHandler : CommandHandlerBase<CarState, CarId>, IRequestHandler<AddItemToCarCommand>
    {
        private readonly ICarPolicy _carPolicy;
        public AddItemToCarCommandHandler(ICarPolicy carPolicy,
            IUnitOfWork unitOfWork,
            IAggregateRootRepository<CarState> rootRepository)
            : base(unitOfWork, rootRepository) =>
            _carPolicy = carPolicy;

        public async Task<Unit> Handle(AddItemToCarCommand request, CancellationToken cancellationToken) =>
            await UpdateAsync(new CarId(request.CarId), async car => Car.AddCarItem(car,
                                                                        new CarItemId(Guid.NewGuid().ToString()), 
                                                                        request.Desc, _carPolicy))
                 .PipeToAsync(Unit.Value);

    }
}
