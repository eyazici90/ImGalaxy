using System;
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public abstract class GivenBase : TestBase
    {
        public void Given(Action action)
        {
            EnsureContainerInitialized();

            action();
        }

        public void Given(Func<Task> givenFunc)
        {
            Given(() => givenFunc().ConfigureAwait(false).GetAwaiter().GetResult());
        }
    }
}
