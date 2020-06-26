using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public interface IProjection<T> where T : class
    {
        ConcurrentDictionary<Type, Collection<Handle<T>>> Handlers { get; }
        Task HandleAsync<TMessage>(TMessage message,
               T connector);
    }
}
