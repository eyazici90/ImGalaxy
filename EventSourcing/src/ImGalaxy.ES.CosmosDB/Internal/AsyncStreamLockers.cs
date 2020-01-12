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

        internal static AsyncSemaphoreSlimWrapper Get(string key) =>
            Lockers.GetOrAdd(key, _ => new AsyncSemaphoreSlimWrapper(new SemaphoreSlim(1, 1)));
    }
}
