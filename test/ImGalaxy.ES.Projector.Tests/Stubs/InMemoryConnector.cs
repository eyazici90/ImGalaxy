using System.Collections.Concurrent;

namespace ImGalaxy.ES.Projector.Tests.Stubs
{
    public class InMemoryConnector
    {
        private readonly static ConcurrentDictionary<string, object> _states = new ConcurrentDictionary<string, object>();

        public void Create<T>(string identifer, T state) =>
            _states.TryAdd($"{typeof(T).Name}-{identifer}", state);


        public T Get<T>(string id) where T : class
        {
            _states.TryGetValue($"{typeof(T).Name}-{id}", out var state);
            return state as T;
        }
    }
}
