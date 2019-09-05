using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryConnection : IInMemoryConnection
    {
        public async Task<IExecutionResult> AppendToStreamAsync(string streamId, long expectedVersion, params InMemoryEventData[] events)
        {
            throw new NotImplementedException();
        }

        public async Task<Optional<InMemoryStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count)
        {
            throw new NotImplementedException();
        }

        public async Task<Optional<InMemoryStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count)
        {
            throw new NotImplementedException();
        }
    }
}
