using Galaxy.Railway; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    public interface IOperationDispatcher
    {
        IExecutionResult RegisterHandler<TOperation>(object handler);

        IExecutionResult RegisterPipeline<TOperation>(object handler);

        Task<IExecutionResult> Dispatch<TOperation>(TOperation operation);

        Task<TResult> Dispatch<TOperation, TResult>(TOperation operation);

        Task<IExecutionResult> Dispatch(object operation);
    }
}
