using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public sealed class CosmosEventData
    {
        public readonly string EventId;
        public readonly string EventType;
        public readonly object Data;
        public readonly EventMetadata EventMetadata;
        public CosmosEventData(string eventId, string eventType, object data, EventMetadata eventMetadata)
        {
            EventId = eventId;
            EventType = eventType;
            Data = data;
            EventMetadata = eventMetadata;
        }
    }
}
