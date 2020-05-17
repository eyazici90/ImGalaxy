using ImGalaxy.ES.CosmosDB.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class ReadStreamWithEventsByDirection
    {
        internal string StreamId { get; }
        internal long Start { get; }
        internal int Count { get; }
        internal Func<string, Task<IEnumerable<EventDocument>>> EventFunc { get; }
        internal ReadStreamWithEventsByDirection(string streamId,
            long start,
            int count,
            Func<string, Task<IEnumerable<EventDocument>>> eventFunc)
        {
            StreamId = streamId;
            Start = start;
            Count = count;
            EventFunc = eventFunc;
        }
    }
}
