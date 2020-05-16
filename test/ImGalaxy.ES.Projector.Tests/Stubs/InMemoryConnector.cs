using ImGalaxy.ES.Projector.Tests.Views; 
using System.Collections.Concurrent; 

namespace ImGalaxy.ES.Projector.Tests.Stubs
{
    public class InMemoryConnector
    {
        private readonly static ConcurrentDictionary<string, CarView> _states = new ConcurrentDictionary<string, CarView>();

        public void Create(CarView state) =>
            _states.TryAdd(state.Id, state);


        public CarView Get(string id)
        {
            _states.TryGetValue(id, out var state);
            return state;
        }
    }
}
