using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryEvent
    {
        public readonly string StreamId;
        public readonly string EventId;
        public readonly long Position;
        public readonly string EventType;
        public readonly InMemoryEventData Data;
        public readonly EventMetadata EventMetadata;
        public readonly DateTime CreatedTime;

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
