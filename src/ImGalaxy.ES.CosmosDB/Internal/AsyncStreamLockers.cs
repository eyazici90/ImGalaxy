using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal static class AsyncStreamLockers
    {
        internal static ConcurrentDictionary<string, AsyncSemaphoreSlimWrapper> Lockers => _lazyLockers.Value;
        
        private static Lazy<ConcurrentDictionary<string, AsyncSemaphoreSlimWrapper>> _lazyLockers =
            new Lazy<ConcurrentDictionary<string, AsyncSemaphoreSlimWrapper>>();

        internal static AsyncSemaphoreSlimWrapper GetOrCreate(string key)
        {
            AsyncSemaphoreSlimWrapper asyncSemaphoreSlimWrapper;

            if (Lockers.TryGetValue(key, out asyncSemaphoreSlimWrapper))
                asyncSemaphoreSlimWrapper.IncreaseRef();

            else
            {
                asyncSemaphoreSlimWrapper = new AsyncSemaphoreSlimWrapper(key, new SemaphoreSlim(1, 1));
                Lockers.TryAdd(key, asyncSemaphoreSlimWrapper);
            }

            return asyncSemaphoreSlimWrapper;
        }
         
        internal static void Release(ref AsyncSemaphoreSlimWrapper asyncSemaphoreSlimWrapper)
        {
            asyncSemaphoreSlimWrapper.DecreaseRef();

            if (asyncSemaphoreSlimWrapper.RefCount <= 0)
                Lockers.TryRemove(asyncSemaphoreSlimWrapper.Key, out var removed);
        }
            
    }
}
