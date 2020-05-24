using FluentAssertions; 
using TestApp.Domain.Cars;
using Xunit;

namespace ImGalaxy.ES.TestApp.Testing
{
    public class ValueObject_Tests
    {
        [Fact]
        public void Should_be_same()
        {
            var newId = "123";

            var first = new CarId(newId);

            var second = new CarId(newId);

            first.Should().BeEquivalentTo(second);

            (first == second).Should().BeTrue();
        }
    }
}
