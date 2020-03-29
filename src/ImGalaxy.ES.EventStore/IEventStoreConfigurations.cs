using EventStore.ClientAPI;
using ImGalaxy.ES.Core; 

namespace ImGalaxy.ES.EventStore
{
    public interface IEventStoreConfigurations : IAggregateStoreConfiguration
    {
        IEventStoreConnection Connection { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Uri { get; set; } 
    }
}
