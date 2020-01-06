using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ImGalaxy.ES.CosmosDB
{
    internal static class AsyncStreamLockers
    {
        internal static ConcurrentDictionary<string, SemaphoreSlim> Lockers => _lazyLockers.Value;
        
        private static Lazy<ConcurrentDictionary<string, SemaphoreSlim>> _lazyLockers =
            new Lazy<ConcurrentDictionary<string, SemaphoreSlim>>();

        internal static SemaphoreSlim Get(string key) =>
            Lockers.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
    }
}
