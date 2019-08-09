using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosStreamNameExtensions
    {
        public static string GetFullStreamName(string type, string identifier) =>
            $"{type}-{identifier}";

        public static string GetStreamType(string streamId) => 
            streamId.Split("-")[0];
        
        public static string GetStreamIdentifier(string streamId) =>
            string.Join("-" , streamId.Split("-").Skip(1).Take(100)); 
        
    }
}
