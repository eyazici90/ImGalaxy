using ImGalaxy.ES.Core;
using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public interface ICommandScenarioFor<TAggregateRootState>
        where TAggregateRootState : IAggregateRootState<TAggregateRootState>, IAggregateRoot
    {
        ICommandScenarioFor<TAggregateRootState> GivenNone();
        ICommandScenarioFor<TAggregateRootState> Given(params object[] events);
        ICommandScenarioFor<TAggregateRootState> WhenNone();
        ICommandScenarioFor<TAggregateRootState> When(Action<TAggregateRootState> command);
        ICommandScenarioFor<TAggregateRootState> When(Func<TAggregateRootState, Task> command);
        ICommandScenarioFor<TAggregateRootState> ThenNone();
        ICommandScenarioFor<TAggregateRootState> Then(params object[] events);
        ICommandScenarioFor<TAggregateRootState> Throws(Exception exception);
        ICommandScenarioFor<TAggregateRootState> Throws(Type exception);
        void Assert();
        void Assert(Action<TAggregateRootState> assertion);

    }
}
