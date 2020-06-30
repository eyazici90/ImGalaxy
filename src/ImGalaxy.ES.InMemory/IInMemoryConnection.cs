using Galaxy.Railway; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{
    public interface IInMemoryConnection
    {
        Task<IExecutionResult> AppendToStreamAsync(string streamId, long expectedVersion, params InMemoryEventData[] events);
        Task<Optional<InMemoryStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count);
        Task<Optional<InMemoryStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count);
    }
}
