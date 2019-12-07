using ImGalaxy.ES.CosmosDB.Documents;
using ImGalaxy.ES.CosmosDB.Enums;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace ImGalaxy.ES.CosmosDB
{
    public static class CosmosConverterExtensions
    {
        public static CosmosStream ToCosmosStream(this StreamDocument document, IEnumerable<EventDocument> eventDocs) =>
             CosmosStream.Create(document.OriginalId, document.Type, document.Version, StreamReadStatus.Success, 
                ReadDirection.Forward,
                eventDocs.Select(e=> CosmosEvent.Create(e.StreamId, e.OriginalId, e.Position, e.Type, e.Data, e.EventMetadata, DateTime.Now))
                    .ToArray());
        public static CosmosStream ToCosmosStream(this StreamDocument document) =>
             CosmosStream.Create(document.OriginalId, document.Type, document.Version, StreamReadStatus.Success,
                ReadDirection.Forward, Array.Empty<CosmosEvent>());

        public static StreamDocument ToCosmosStreamDocument(this CosmosStream stream) =>
             new StreamDocument(stream.Id, stream.Version, stream.Version, stream.Type);
        
        public static CosmosEvent ToCosmosEvent(this EventDocument document) =>
             CosmosEvent.Create(document.StreamId, document.OriginalId, document.Position, document.Type,
                document.Data, document.EventMetadata, DateTime.Now);
        
        public static EventDocument ToCosmosEventDocument(this CosmosEvent document) =>
             new EventDocument(document.EventId, document.StreamId, document.Position,
                document.Data, document.EventMetadata, document.EventType);
        

    }
}
