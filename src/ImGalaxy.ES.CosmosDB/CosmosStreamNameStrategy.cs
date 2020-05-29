using System.Linq; 

namespace ImGalaxy.ES.CosmosDB
{
    public static class CosmosStreamNameStrategy
    {
        public static string GetFullStreamName(string type, string identifier) =>
            $"{type}-{identifier}";

        public static string GetStreamType(string streamId) => 
            streamId.Split("-")[0];
        
        public static string GetStreamIdentifier(string streamId) =>
            string.Join("-" , streamId.Split("-").Skip(1).Take(100)); 
        
    }
}
