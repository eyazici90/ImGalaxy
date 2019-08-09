using ImGalaxy.ES.CosmosDB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosStream
    {
        public readonly string Id;
        public readonly string Type;
        public readonly long Version;
        public readonly StreamReadStatus StreamReadStatus;
        public readonly ReadDirection ReadDirection;
        public readonly CosmosEvent[] Events;
        public readonly long LastEventNumber;
        public readonly long NextEventNumber;

        public CosmosStream(string id, string type, long version, StreamReadStatus streamReadStatus,
            ReadDirection readDirection, CosmosEvent[] events)
        {
            Id = id;
            Type = type;
            Version = version;
            StreamReadStatus = streamReadStatus;
            ReadDirection = readDirection;
            Events = events;
            LastEventNumber = Events.Length;
            NextEventNumber = LastEventNumber + 1;
        }

        public CosmosStream AppendEvents(IEnumerable<CosmosEvent> cosmosEvents) =>
             new CosmosStream(this.Id, this.Type, this.Version, this.StreamReadStatus, this.ReadDirection, cosmosEvents.ToArray());
        
    }
}
