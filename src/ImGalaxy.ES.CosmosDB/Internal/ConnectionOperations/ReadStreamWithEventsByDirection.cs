using ImGalaxy.ES.CosmosDB.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class ReadStreamWithEventsByDirection
    {
        internal string StreamId { get; }
        internal long Start { get; }
        internal int Count { get; }
        internal Func<string, IEnumerable<EventDocument>> EventFunc { get; }
        internal ReadStreamWithEventsByDirection(string streamId,
            long start,
            int count,
            Func<string, IEnumerable<EventDocument>> eventFunc)
        {
            StreamId = streamId;
            Start = start;
            Count = count;
            EventFunc = eventFunc;
        }
    }
}
