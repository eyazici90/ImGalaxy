using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmoStore
{
    public interface ICosmoConfigurator
    { 
        string EndpointUri { get; set; }
        string PrimaryKey { get; set; }
        string DatabaseId { get; set; }
    }
}
