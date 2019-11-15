using ImGalaxy.ES.CosmosDB.Enums;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Documents
{
    public abstract class BaseDocument 
    {
        public readonly string OriginalId; 
        public readonly string id;
        public readonly string Type;
        public BaseDocument(string originalId, string type)
        {
            OriginalId = originalId;
            id = OriginalId;
            Type = type;    
        }
    }
}
