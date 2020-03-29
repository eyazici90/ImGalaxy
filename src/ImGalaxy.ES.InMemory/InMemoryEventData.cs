using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory
{ 
    public sealed class InMemoryEventData
    {
        public string EventId { get; }
        public string EventType { get; }
        public object Data { get; }
        public EventMetadata EventMetadata { get; }
        public InMemoryEventData(string eventId, string eventType, object data, EventMetadata eventMetadata)
        {
            EventId = eventId;
            EventType = eventType;
            Data = data;
            EventMetadata = eventMetadata;
        }
    }
}
