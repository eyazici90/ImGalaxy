using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class AppendToStreamAsync
    {
        internal string StreamId { get; }
        internal Version ExpectedVersion { get; set; }
        internal CosmosEventData[] Events { get; }
        internal AppendToStreamAsync(string streamId,
            Version expectedVersion,
            CosmosEventData[] events)
        {
            StreamId = streamId;
            ExpectedVersion = expectedVersion;
            Events = events;
        }
    }
}
