﻿using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public class EventDocument : BaseDocument
    {
        public readonly string StreamId;
        public readonly long Position;
        public readonly string Data;
        public readonly EventMetadata EventMetadata;
        public EventDocument(string originalId, string streamId, long position, string data,
            EventMetadata eventMetadata, string type) : base(originalId, type)
        {
            StreamId = streamId;
            Position = position;
            Data = data;
            EventMetadata = eventMetadata;
        }
    }
}
