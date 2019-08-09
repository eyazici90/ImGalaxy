using ImGalaxy.ES.CosmosDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public abstract class BaseDocument
    {
        public readonly string Id;
        public readonly string Type;
        public BaseDocument(string id, string type)
        {
            Id = id;
            Type = type;    
        }
    }
}
