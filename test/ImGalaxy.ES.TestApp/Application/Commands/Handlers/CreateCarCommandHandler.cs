using System.Threading;
using System.Threading.Tasks;
using Galaxy.Railway;
using ImGalaxy.ES.Core;
using MediatR;
using TestApp.Domain.Cars;
using Unit = MediatR.Unit;

namespace TestApp.Application.Commands.Handlers
{
    public sealed class CreateCarCommandHandler : CommandHandlerBase<CarState, CarId>,
        IRequestHandler<CreateCarCommand>
    {
        public CreateCarCommandHandler(IUnitOfWork unitOfWork, IAggregateRootRepository<CarState> rootRepository)
            : base(unitOfWork, rootRepository)
        {
        }

        public async Task<Unit> Handle(CreateCarCommand request, CancellationToken cancellationToken) =>
             await AddAsync(async () => Car.RegisterCar(request.Id, request.Name).State, request.Id)
                  .MapAsync(_ => Unit.Value);

    }
}
