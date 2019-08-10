using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public class SnapshotDocument : BaseDocument
    {
        public SnapshotDocument(string originalId, string type)
            : base(originalId, type)
        {
        }
    }
}
