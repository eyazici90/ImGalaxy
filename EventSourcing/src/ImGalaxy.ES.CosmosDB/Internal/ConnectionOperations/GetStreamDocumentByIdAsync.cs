using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class GetStreamDocumentByIdAsync
    {
        internal string Id { get; }
        internal GetStreamDocumentByIdAsync(string id)
        {
            Id = id;
        }
    }
}
