using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.TestBase
{
    public abstract class GivenWhenThen : GivenBase
    {
        private Func<object, Task> _whenLaterResultAsync;

        private Func<Task> _whenLaterAsync;

        public Func<object, Task> WhenResultAction
        {
            get { return _whenLaterResultAsync; }
            set
            {
                EnsureContainerInitialized();
                _whenLaterResultAsync = value;
            }
        }
        public Func<Task> WhenAction
        {
            get { return _whenLaterAsync; }
            set
            {
                EnsureContainerInitialized();
                _whenLaterAsync = value;
            }
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
        protected virtual void WhenLater(Func<Task> whenLaterFunc)
        {
            this.WhenAction = async () => await whenLaterFunc();
        }
        protected virtual void WhenLater<TCommand>(TCommand cmd, Func<TCommand, Task> whenLaterFunc)
        {
            this.WhenResultAction = async c => await whenLaterFunc(cmd);
        }

    }
}
