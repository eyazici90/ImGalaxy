using ImGalaxy.ES.Core;
using System;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal interface IOperationPipeline<TOperation> : IOperationPipeline<TOperation, IExecutionResult>
    {
    }

    internal interface IOperationPipeline<TOperation, TResult>
    {
        Task<TResult> Handle(TOperation operation, Func<TOperation, Task<TResult>> next); 
    }
}
