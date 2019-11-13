using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ImGalaxy.ES.CosmosDB
{
    public static class AsyncStreamLockers
    {
        public static ConcurrentDictionary<string, SemaphoreSlim> Lockers { get; } 
            = new ConcurrentDictionary<string, SemaphoreSlim>();

        public static SemaphoreSlim Get(string key)=>
            Lockers.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
    }
}
