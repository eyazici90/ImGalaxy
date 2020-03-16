using System; 
using System.Threading.Tasks;
using static ImGalaxy.ES.Projector.ProjectorDelegates;

namespace ImGalaxy.ES.Projector
{
    public interface IProjection<T> where T : class
    {
        Task HandleAsync<TMessage>(UpdateOrInsert updateOrInsert,
            TMessage message,
            T state,
            Type type);
    }
}
