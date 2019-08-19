using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImGalaxy.ES.Core;
using MediatR;
using TestCosmos.Domain.Cars;

namespace TestCosmos.Application.Commands.Handlers
{
    public sealed class CreateCarCommandHandler : CommandHandlerBase<CarState, CarId>, IRequestHandler<CreateCarCommand>
    {
        public CreateCarCommandHandler(IUnitOfWork unitOfWork, IAggregateRootRepository<CarState> rootRepository) 
            : base(unitOfWork, rootRepository)
        {
        }

        public async Task<Unit> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            var newId = Guid.NewGuid().ToString();

            return await AddAsync(async () => Car.RegisterCar(newId, request.Name).State, newId)
                .PipeToAsync(Unit.Value);
        }
           
    }
}
