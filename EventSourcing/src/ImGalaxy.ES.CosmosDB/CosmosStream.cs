using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Version = ImGalaxy.ES.Core.Version;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosStream
    {
        public string Id { get; }
        public string Type { get; }
        public Version Version { get; }
        public StreamReadStatus StreamReadStatus { get; }
        public ReadDirection ReadDirection { get; }
        public CosmosEvent[] Events { get; }
        public long LastEventNumber { get; }
        public long NextEventNumber { get; }

        private CosmosStream(string id, string type, Version version, StreamReadStatus streamReadStatus,
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
        public static CosmosStream Create(string id, string type, Version version, StreamReadStatus streamReadStatus,
            ReadDirection readDirection, CosmosEvent[] events) =>
            new CosmosStream(id, type, version, streamReadStatus, readDirection, events);

        public static CosmosStream Create(string id, string type) =>
           new CosmosStream(id, type, ExpectedVersion.New, StreamReadStatus.Success, ReadDirection.Forward, new CosmosEvent[0]);

        public CosmosStream AppendEvents(IEnumerable<CosmosEvent> cosmosEvents) =>
             new CosmosStream(this.Id, this.Type, this.Version, this.StreamReadStatus, this.ReadDirection, cosmosEvents.ToArray());

        public CosmosStream ChangeVersion(Version version) =>
            new CosmosStream(this.Id, this.Type, version, this.StreamReadStatus, this.ReadDirection, this.Events);


    }
}
