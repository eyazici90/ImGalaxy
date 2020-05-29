using ImGalaxy.ES.Core; 

namespace ImGalaxy.ES.CosmosDB
{
    public sealed class CosmosEventData
    {
        public string EventId { get; }
        public string EventType { get; }
        public object Data { get; }
        public EventMetadata EventMetadata { get; }
        public CosmosEventData(string eventId, string eventType, object data, EventMetadata eventMetadata)
        {
            EventId = eventId;
            EventType = eventType;
            Data = data;
            EventMetadata = eventMetadata;
        }
    }
}
