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

        private CosmosStream(string id, string type, long version, StreamReadStatus streamReadStatus,
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
        public static CosmosStream Create(string id, string type, long version, StreamReadStatus streamReadStatus,
            ReadDirection readDirection, CosmosEvent[] events) =>
            new CosmosStream(id, type, version, streamReadStatus, readDirection, events);

        public static CosmosStream Create(string id, string type) =>
           new CosmosStream(id, type, 0, StreamReadStatus.Success, ReadDirection.Forward, new CosmosEvent[0]);

        public CosmosStream AppendEvents(IEnumerable<CosmosEvent> cosmosEvents) =>
             new CosmosStream(this.Id, this.Type, this.Version, this.StreamReadStatus, this.ReadDirection, cosmosEvents.ToArray());

        public CosmosStream ChangeVersion(long version) =>
            new CosmosStream(this.Id, this.Type, version, this.StreamReadStatus, this.ReadDirection, this.Events);


    }
}
