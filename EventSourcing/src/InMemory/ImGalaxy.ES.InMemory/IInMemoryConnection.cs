using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
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
