using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal class AsyncSemaphoreSlimWrapper
    {
        internal SemaphoreSlim SemaphoreSlim { get; }
        internal int RefCount { get; private set; }
        internal AsyncSemaphoreSlimWrapper(SemaphoreSlim semaphoreSlim)
        {
            SemaphoreSlim = semaphoreSlim;
            RefCount = 1;
        }

        internal void IncreaseRef() => RefCount++;

        internal void DecreaseRef() => RefCount--;
    }
}
