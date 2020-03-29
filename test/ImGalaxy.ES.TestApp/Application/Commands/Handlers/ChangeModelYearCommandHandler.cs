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
    public class ChangeModelYearCommandHandler : CommandHandlerBase<CarState, CarId>, IRequestHandler<ChangeModelYearCommand>
    {
        private readonly ICarPolicy _carPolicy;
        public ChangeModelYearCommandHandler(ICarPolicy carPolicy,
            IUnitOfWork unitOfWork,
            IAggregateRootRepository<CarState> rootRepository)
            : base(unitOfWork, rootRepository) =>
            _carPolicy = carPolicy;


        public async Task<Unit> Handle(ChangeModelYearCommand request, CancellationToken cancellationToken) =>
            await UpdateAsync(new CarId(request.CarId), async car => Car.RenewModel(car, request.Year, _carPolicy))
                 .PipeToAsync(Unit.Value);

    }
}
