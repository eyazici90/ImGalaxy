using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using static ImGalaxy.ES.Projector.ProjectorDelegates;

namespace ImGalaxy.ES.Projector
{
    public abstract class Projection<T> : IProjection<T>
      where T : class
    {
        private readonly Dictionary<Type, Func<object, T, Task<T>>> _handlers = new Dictionary<Type, Func<object, T, Task<T>>>();

        protected virtual void When<TMessage>(Func<TMessage, T, Task<T>> handler)
         where TMessage : class
            => _handlers.Add(typeof(TMessage), async (@event, state) => await handler(@event as TMessage, state));

        protected virtual void When<TMessage>(Func<TMessage, T, Task> handler)
         where TMessage : class
            => _handlers.Add(typeof(TMessage), async (@event, state)
                =>
            {
                await handler(@event as TMessage, state);
                return state;
            });

        public virtual async Task HandleAsync<TMessage>(UpdateOrInsert updateOrInsert,
            TMessage message,
            T state,
            Type type)
        {
            _handlers.TryGetValue(type, out var handler);

            if (handler == default)
                throw new ArgumentNullException($"handler type could not found of {type.Name}");

            var newState = await handler(message, state);

            await updateOrInsert(newState);
        }
    }
}
