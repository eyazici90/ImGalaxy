using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal interface IOperationPipeline<TOperation>
    {
        Task Next(TOperation operation);
    }
}
