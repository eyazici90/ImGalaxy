using Newtonsoft.Json; 

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public class StreamDocument : BaseDocument
    {
        public readonly long StreamPosition;
        public readonly long Version;

        [JsonProperty("_etag")]
        public readonly string Etag;
        public StreamDocument(string originalId, long streamPosition, 
            long version, string type, string etag) 
            : base(originalId, type)
        {
            StreamPosition = streamPosition;
            Version = version;
            Etag = etag;
        }
    }
}
