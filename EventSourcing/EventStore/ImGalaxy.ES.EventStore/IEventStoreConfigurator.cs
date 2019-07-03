using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public interface IEventStoreConfigurator
    {
        IEventStoreConnection Connection { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Uri { get; set; }
        bool IsSnapshottingOn { get; set; }
    }
}
