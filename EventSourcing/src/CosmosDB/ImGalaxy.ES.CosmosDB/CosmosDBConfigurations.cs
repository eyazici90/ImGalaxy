using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBConfigurations : ICosmosDBConfigurations
    {
        public string EndpointUri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string PrimaryKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DatabaseId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MaxLiveQueueSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ReadBatchSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsSnapshottingOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string StreamCollectionName => throw new NotImplementedException();

        public string EventCollectionName => throw new NotImplementedException();
    }
}
