using FluentAssertions;
using ImGalaxy.ES.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public class CommandScenarioFor<TAggregateRootState> : ICommandScenarioFor<TAggregateRootState>
      where TAggregateRootState : class, IAggregateRootState<TAggregateRootState>, IAggregateRoot
    {
        public static CommandScenarioFor<TAggregateRootState> With(TAggregateRootState sut) => new CommandScenarioFor<TAggregateRootState>(sut);
        public static CommandScenarioFor<TAggregateRootState> With(Task<TAggregateRootState> sut) => With(sut.ConfigureAwait(false).GetAwaiter().GetResult());
        public static CommandScenarioFor<TAggregateRootState> With(Func<TAggregateRootState> sutFactory) => new CommandScenarioFor<TAggregateRootState>(sutFactory);

        public readonly Func<TAggregateRootState> _sutFactory;
        private TAggregateRootState _sut;
        private object[] _givenEvents;
        private object[] _thenEvents;
        private Exception throwenException;
        private Exception expectedException;
        public CommandScenarioFor(TAggregateRootState sut)
          : this(() => sut) { }

        public CommandScenarioFor(Func<TAggregateRootState> sutFactory)
        {
            if (sutFactory == null) throw new ArgumentNullException(nameof(sutFactory));
            _sutFactory = () => sutFactory();
            _sut = _sutFactory();
        }

        public ICommandScenarioFor<TAggregateRootState> GivenNone()
        {
            _givenEvents = null;
            return this;
        }

        public ICommandScenarioFor<TAggregateRootState> Given(params object[] events)
        {
            _givenEvents = events;
            return this;
        }
        public ICommandScenarioFor<TAggregateRootState> WhenNone()
        {
            return this;
        }
        public ICommandScenarioFor<TAggregateRootState> When(Action<TAggregateRootState> command)
        {
            try
            {
                command(_sut);
                throwenException = null;
            }
            catch (Exception ex)
            {
                throwenException = ex;
            }


            return this;
        }
        public ICommandScenarioFor<TAggregateRootState> When(Func<TAggregateRootState, Task> command) =>
            When(state =>
                command(state)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult());


        public ICommandScenarioFor<TAggregateRootState> ThenNone()
        {
            _thenEvents = null;
            return this;
        }
        public ICommandScenarioFor<TAggregateRootState> Then(params object[] events)
        {
            _thenEvents = events;
            return this;
        }
        public void Assert()
        {
            var changeEventsForsut = ((IAggregateRootChangeTracker)_sut).GetEvents();
            if (throwenException != null)
            {
                expectedException.GetType().Should().Be(throwenException.GetType());
                return;
            }
            changeEventsForsut.Should().BeEquivalentTo(_thenEvents);
            changeEventsForsut.Select(e => e.GetType()).Should().BeEquivalentTo(_thenEvents.Select(e => e.GetType()));
        }

        public void Assert(Action<TAggregateRootState> assertion)
        {
            assertion(_sut);
        }

        public ICommandScenarioFor<TAggregateRootState> Throws(Exception exception)
        {
            expectedException = exception;
            return this;
        }
        public ICommandScenarioFor<TAggregateRootState> Throws(Type exception)
        {
            expectedException = Activator.CreateInstance(exception) as Exception;
            return this;
        }
    }
}
