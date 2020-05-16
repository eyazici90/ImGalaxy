using FluentAssertions;
using ImGalaxy.ES.Projector.Tests.Projections;
using ImGalaxy.ES.Projector.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using System; 
using System.Threading.Tasks;
using TestApp;
using Xunit;

namespace ImGalaxy.ES.Projector.Tests
{
    public class Projector_Tests
    {

        [Fact]
        public async Task Should_be_created_when_projected()
        {
            //Arrange
            var connector = new InMemoryConnector();

            var projector = new ConnectedProjector<InMemoryConnector>(connector, _ => new CarProjection());

            var @event = new CarRegisteredEvent("123", "Ferrari");

            //Act
            await projector.ProjectAsync(@event).ConfigureAwait(false);

            //Assertion
            var view = connector.Get("123");
            view.Should().NotBeNull();

            view.Name.Should().Be("Ferrari");

        }


        [Fact]
        public void Should_throw_when_projected()
        {
            //Arrange
            var connector = new InMemoryConnector();

            var projector = new ConnectedProjector<InMemoryConnector>(connector, _ => null);

            var @event = new CarRegisteredEvent("123", "Ferrari");

            //Act
            Func<Task> act = () => projector.ProjectAsync(@event);

            act.Should().Throw<ProjectionNotFoundException>();
        }


        [Theory]
        [InlineData(typeof(IProjector))]
        [InlineData(typeof(IProjection<InMemoryConnector>))]
        public void Should_not_be_null_when_resolved(Type type)
        {
            var services = new ServiceCollection();

            services.AddSingleton<InMemoryConnector>();

            services.AddProjector<InMemoryConnector>(typeof(CarProjection));

            var provider = services.BuildServiceProvider();

            var @object = provider.GetService(type);

            @object.Should().NotBeNull();

            @object.Should().BeAssignableTo(type);
        }
    }
}
