using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal struct AsyncSemaphoreSlimWrapper
    {
        internal string Key { get; }
        internal SemaphoreSlim SemaphoreSlim { get; }
        internal int RefCount { get; private set; }
        internal AsyncSemaphoreSlimWrapper(string key,
            SemaphoreSlim semaphoreSlim)
        {
            Key = key;
            SemaphoreSlim = semaphoreSlim;
            RefCount = 1;
        }

        internal void IncreaseRef() => RefCount++;

        internal void DecreaseRef() => RefCount--;
    }
}
