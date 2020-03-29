using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public abstract class GivenWhenThen : TestBase
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

        protected virtual void When(Func<Task> whenFunc)
        {
            EnsureContainerInitialized();

            whenFunc()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        protected virtual TResult When<TResult>(Func<Task<TResult>> whenFunc)
        {
            EnsureContainerInitialized();

            return whenFunc()
                     .ConfigureAwait(false)
                     .GetAwaiter()
                     .GetResult();
        }


        protected virtual void When<TCommand>(TCommand cmd, Func<TCommand, Task> whenFunc)
        {
            EnsureContainerInitialized();

            whenFunc(cmd)
             .ConfigureAwait(false)
             .GetAwaiter()
             .GetResult();
        } 
    }
}
