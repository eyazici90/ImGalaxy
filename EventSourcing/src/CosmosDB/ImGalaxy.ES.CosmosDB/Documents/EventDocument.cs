using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public class EventDocument : BaseDocument
    {
        public readonly string StreamId;
        public readonly long Position; 
        public readonly object Data;
        public readonly EventMetadata EventMetadata;
        public EventDocument(string id, string streamId, long position, object data
            , EventMetadata eventMetadata, string type ) 
            : base(id, type)
        {
            StreamId = streamId;
            Position = position; 
            Data = data;
            EventMetadata = eventMetadata;
        }
    }
}
