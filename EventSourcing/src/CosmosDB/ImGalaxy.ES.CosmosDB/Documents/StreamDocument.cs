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
        public StreamDocument(string originalId, long streamPosition, 
            long version, string type) 
            : base(originalId, type)
        {
            StreamPosition = streamPosition;
            Version = version;
        }
    }
}
