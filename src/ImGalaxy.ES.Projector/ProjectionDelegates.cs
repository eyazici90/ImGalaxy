using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public delegate Task Handle<T>(object @event, T connector) where T : class;
}
