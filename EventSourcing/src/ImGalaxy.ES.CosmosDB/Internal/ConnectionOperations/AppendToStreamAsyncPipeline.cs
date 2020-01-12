using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class AppendToStreamAsyncPipeline : IOperationPipeline<AppendToStreamAsync>
    {
        async Task<IExecutionResult> IOperationPipeline<AppendToStreamAsync, IExecutionResult>.Handle(AppendToStreamAsync operation, Func<AppendToStreamAsync, Task<IExecutionResult>> next)
        {
            var asyncLockerWrapper = AsyncStreamLockers.GetOrCreate(operation.StreamId);

            await asyncLockerWrapper.SemaphoreSlim.LockAsync(async () =>
                await next(operation)
            );

            AsyncStreamLockers.Release(ref asyncLockerWrapper);

            return ExecutionResult.Success; 
        }
    }
}
