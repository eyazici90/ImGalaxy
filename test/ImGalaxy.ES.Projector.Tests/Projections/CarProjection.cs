using ImGalaxy.ES.Projector.Tests.Stubs;
using ImGalaxy.ES.Projector.Tests.Views;
using TestApp;

namespace ImGalaxy.ES.Projector.Tests.Projections
{
    public class CarProjection : Projection<InMemoryConnector>
    {
        public CarProjection()
        {
            When<CarRegisteredEvent>(async (@event, connector) =>
            {
                connector.Create(@event.Id, new CarView { Id = @event.Id, Name = @event.Name });
            });
        }
    }
}
