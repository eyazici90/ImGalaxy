using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryEvent
    {
        public string StreamId { get; }
        public string EventId { get; }
        public long Position { get; }
        public string EventType { get; }
        public InMemoryEventData Data { get; }
        public EventMetadata EventMetadata { get; }
        public DateTime CreatedTime { get; }

        private InMemoryEvent(string streamId, string eventId, long position,
            string eventType, InMemoryEventData data, EventMetadata eventMetadata, DateTime createdTime)
        {
            StreamId = streamId;
            EventId = eventId;
            Position = position;
            EventType = eventType;
            Data = data;
            EventMetadata = eventMetadata;
            CreatedTime = createdTime;
        }
        public static InMemoryEvent Create(string streamId, string eventId, long position,
            string eventType, InMemoryEventData data, EventMetadata eventMetadata, DateTime createdTime) =>
            new InMemoryEvent(streamId, eventId, position,
            eventType, data, eventMetadata, createdTime);
    }
}
