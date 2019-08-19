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
    public class ChangeModelYearCommandHandler : CommandHandlerBase<CarState, CarId>, IRequestHandler<ChangeModelYearCommand>
    {
        public ChangeModelYearCommandHandler(IUnitOfWork unitOfWork,
            IAggregateRootRepository<CarState> rootRepository)
            : base(unitOfWork, rootRepository)
        {
        }

        public async Task<Unit> Handle(ChangeModelYearCommand request, CancellationToken cancellationToken)=>
            await UpdateAsync(new CarId(request.CarId), async car => Car.RenewModel(car, request.Year))
                 .PipeToAsync(Unit.Value);
        
    }
}
