using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public interface ICosmosDBConnection
    {
        Task AppendToStreamAsync(string streamId, long expectedVersion, params CosmosEventData[] events);
        Task<Optional<CosmosStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count);
        Task<Optional<CosmosStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count); 
    }
}
