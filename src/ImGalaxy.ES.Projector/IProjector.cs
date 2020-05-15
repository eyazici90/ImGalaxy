using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public interface IProjector
    {
        Task ProjectAsync(object @event);
    }
}
