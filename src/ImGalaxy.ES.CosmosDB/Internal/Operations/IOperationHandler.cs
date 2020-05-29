using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal interface IOperationHandler<TOperation,  TResult> 
    {
        Task<TResult> Handle(TOperation operation, CancellationToken cancellationToken = default);
    } 

    internal interface IOperationHandler<TOperation> : IOperationHandler<TOperation, IExecutionResult>
    { 
    }

    internal interface IOperationHandler : IOperationHandler<object>
    {
    }
}
