using ImGalaxy.ES.Projector.Tests.Stubs;
using ImGalaxy.ES.Projector.Tests.Views;
using TestApp;

namespace ImGalaxy.ES.Projector.Tests.Projections
{
    public class CarHistoryProjection : Projection<InMemoryConnector>
    {
        public CarHistoryProjection()
        {
            When<CarRegisteredEvent>(async (@event, connector) =>
            {
                connector.Create(@event.Id, new CarHistoryView { Id = @event.Id, Name = @event.Name });
            });

            When<CarModelRenewedEvent>(async (@event, connector) =>
            {
                connector.Create(@event.Id, new CarHistoryView { Id = @event.Id, Name = @event.Name });
            });
        }
    }
}
