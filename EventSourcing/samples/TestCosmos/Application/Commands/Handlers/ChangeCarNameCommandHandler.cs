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
    public class ChangeCarNameCommandHandler : CommandHandlerBase<CarState, CarId>, IRequestHandler<ChangeCarNameCommand>
    {
        public ChangeCarNameCommandHandler(IUnitOfWork unitOfWork, 
            IAggregateRootRepository<CarState> rootRepository) 
            : base(unitOfWork, rootRepository)
        {
        }

        public async Task<Unit> Handle(ChangeCarNameCommand request, CancellationToken cancellationToken)=>
            await UpdateAsync(new CarId(request.CarId), async car => Car.ChangeName(car, request.Name))
                .PipeToAsync(Unit.Value);
        
    }
}
