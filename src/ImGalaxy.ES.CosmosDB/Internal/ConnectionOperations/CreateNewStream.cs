using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class CreateNewStream 
    {
        internal string Id { get; }
        internal string StreamType { get; }
        internal CosmosEventData[] Events { get; }
        internal CreateNewStream(string id,
            string streamType,
            CosmosEventData[] events)
        {
            Id = id;
            StreamType = streamType;
            Events = events;
        }
    }
}
