using ImGalaxy.ES.Core; 
using System.Threading;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal interface IOperationHandler<TOperation>
    {
        Task<IExecutionResult> Handle(TOperation operation, CancellationToken cancellationToken = default);
    }
}
