using System; 
using System.Threading;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public static class AsyncExtensions
    {
        public static async Task LockAsync(this SemaphoreSlim @semaphore, Func<Task> task)
        {
            await @semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                await task();
            }
            finally 
            {
                @semaphore.Release();
            }
        }

        public static void Lock(this SemaphoreSlim @semaphore, Action action)
        {
            @semaphore.Wait();
            try
            {
                action();
            }
            finally
            {
                @semaphore.Release();
            }
        }
    }
}
