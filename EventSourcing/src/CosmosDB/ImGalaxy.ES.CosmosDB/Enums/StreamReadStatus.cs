using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Enums
{ 
    public enum StreamReadStatus
    {
        Success = 0,
        StreamNotFound = 1,
        StreamDeleted = 2
    }
}
