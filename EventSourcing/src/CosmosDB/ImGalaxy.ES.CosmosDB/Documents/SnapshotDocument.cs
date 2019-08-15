using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public class SnapshotDocument : BaseDocument
    {
        public readonly string StreamId;
        public readonly string State;
        public readonly string EventPosition;
        public SnapshotDocument(string streamId, string state, string eventPosition,
            string originalId, string type)
            : base(originalId, type)
        {
            StreamId = streamId;
            State = state;
            EventPosition = eventPosition;
        }


    }
}
