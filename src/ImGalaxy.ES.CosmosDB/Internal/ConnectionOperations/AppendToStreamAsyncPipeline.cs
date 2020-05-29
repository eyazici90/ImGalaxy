using System; 
using System.Threading.Tasks;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class AppendToStreamAsyncPipeline : IOperationPipeline<AppendToStreamAsync>
    {  
        async Task<IExecutionResult> IOperationPipeline<AppendToStreamAsync, IExecutionResult>.Handle(AppendToStreamAsync operation, Func<AppendToStreamAsync, Task<IExecutionResult>> next)
        {
            var asyncLockerWrapper = AsyncStreamLockers.GetOrCreate(operation.StreamId);

            await asyncLockerWrapper.SemaphoreSlim.LockAsync(async () => await next(operation).ConfigureAwait(false)).ConfigureAwait(false);

            AsyncStreamLockers.Release(ref asyncLockerWrapper);

            return ExecutionResult.Success; 
        }
    }
}
