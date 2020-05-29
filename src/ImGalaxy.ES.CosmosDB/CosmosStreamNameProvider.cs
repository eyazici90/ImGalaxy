using ImGalaxy.ES.Core;
using System; 

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosStreamNameProvider : IStreamNameProvider
    {
        public string GetSnapshotStreamName(object aggregateRoot, string identifier) =>
              identifier;
        public string GetSnapshotStreamName(Type aggregateRootType, string identifier) =>
              identifier;

        public string GetStreamName(object aggregateRoot, string identifier) =>
            CosmosStreamNameStrategy.GetFullStreamName(aggregateRoot.GetType().ToString(), identifier);
        public string GetStreamName(Type aggregateRootType, string identifier)=>
            CosmosStreamNameStrategy.GetFullStreamName(aggregateRootType.ToString(), identifier);
    }
}
