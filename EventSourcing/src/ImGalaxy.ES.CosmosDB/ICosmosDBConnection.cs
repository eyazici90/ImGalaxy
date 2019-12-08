using ImGalaxy.ES.Core; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBConnection
    {
        Task<IExecutionResult> AppendToStreamAsync(string streamId, Version expectedVersion, params CosmosEventData[] events);
        Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count);
        Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count); 
    }
}
