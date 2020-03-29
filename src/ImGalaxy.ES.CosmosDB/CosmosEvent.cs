using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosEvent
    {
        public string StreamId { get; }
        public string EventId { get; }
        public long Position { get; }
        public string EventType { get; }
        public string Data { get; }
        public EventMetadata EventMetadata { get; }
        public DateTime CreatedTime { get; }

        private CosmosEvent(string streamId, string eventId, long position,
            string eventType, string data, EventMetadata eventMetadata, DateTime createdTime)
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
            string eventType, string data, EventMetadata eventMetadata, DateTime createdTime) =>
            new CosmosEvent(streamId, eventId, position,
            eventType, data, eventMetadata, createdTime);


    }
}
