using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosEvent
    {
        public readonly string StreamId;
        public readonly string EventId;
        public readonly long Position;
        public readonly string EventType;
        public readonly object Data;
        public readonly EventMetadata EventMetadata;
        public readonly DateTime CreatedTime;

        private CosmosEvent(string streamId, string eventId, long position,
            string eventType, object data, EventMetadata eventMetadata, DateTime createdTime)
        {
            StreamId = streamId;
            EventId = eventId;
            Position = position;
            EventType = eventType;
            Data = data;
            EventMetadata = eventMetadata;
            CreatedTime = createdTime;
        }
        public static CosmosEvent Create(string streamId, string eventId, long position,
            string eventType, object data, EventMetadata eventMetadata, DateTime createdTime) =>
            new CosmosEvent(streamId, eventId, position,
            eventType, data, eventMetadata, createdTime);


    }
}
