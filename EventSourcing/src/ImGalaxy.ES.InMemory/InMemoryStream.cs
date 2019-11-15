using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryStream
    {
        public readonly string Id;
        public readonly string Type;
        public readonly long Version; 
        public readonly InMemoryEvent[] Events;
        public readonly long LastEventNumber;
        public readonly long NextEventNumber;

        private InMemoryStream(string id, string type, long version, InMemoryEvent[] events)
        {
            Id = id;
            Type = type;
            Version = version; 
            Events = events;
            LastEventNumber = Events.Length;
            NextEventNumber = LastEventNumber + 1;
        }
        public static InMemoryStream Create(string id, string type, long version, InMemoryEvent[] events) =>
            new InMemoryStream(id, type, version, events);

        public static InMemoryStream Create(string id, string type) =>
           new InMemoryStream(id, type, 1, new InMemoryEvent[0]);

        public InMemoryStream AppendEvents(IEnumerable<InMemoryEvent> cosmosEvents) =>
             new InMemoryStream(this.Id, this.Type, this.Version, cosmosEvents.ToArray());

        public InMemoryStream ChangeVersion(long version) =>
            new InMemoryStream(this.Id, this.Type, version, this.Events);
    }
}
