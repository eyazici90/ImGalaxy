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
