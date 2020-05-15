using System.Threading.Tasks; 

namespace ImGalaxy.ES.Projector
{
    public interface IProjection<T> where T : class
    {
        Task HandleAsync<TMessage>(TMessage message,
               T connector);
    }
}
