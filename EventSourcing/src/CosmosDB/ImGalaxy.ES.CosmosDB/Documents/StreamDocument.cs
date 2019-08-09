using ImGalaxy.ES.CosmosDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public class StreamDocument : BaseDocument
    {
        public readonly long StreamPosition;
        public readonly long Version;
        public StreamDocument(string id, long streamPosition, long version, string type) : base(id, type)
        {
            StreamPosition = streamPosition;
            Version = version;
        }
    }
}
