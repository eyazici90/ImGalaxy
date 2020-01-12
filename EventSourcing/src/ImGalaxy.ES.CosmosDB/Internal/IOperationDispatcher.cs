using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    public interface IOperationDispatcher
    {
        IExecutionResult RegisterHandler<TOperation>(Func<object> handler);

        IExecutionResult RegisterPipeline<TOperation>(Func<object> handler);

        Task<IExecutionResult> Dispatch<TOperation>(TOperation operation);

        Task<TResult> Dispatch<TOperation, TResult>(TOperation operation);

        Task<IExecutionResult> Dispatch(object operation);
    }
}
