using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public interface IProjector
    {
        Task ProjectAsync<T>(object @event)
            where T : class;
        Task ProjectAsync<T>(string id,
            object @event) where T : class;
    }
}
