using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public abstract class Projection<T> : IProjection<T> where T : class
    {
        public readonly ConcurrentDictionary<Type, Collection<Handle<T>>> _handlers = new ConcurrentDictionary<Type, Collection<Handle<T>>>();
        public ConcurrentDictionary<Type, Collection<Handle<T>>> Handlers => _handlers; 
        protected virtual void When<TMessage>(Func<TMessage, T, Task> handler)
         where TMessage : class =>
           _handlers.AddOrUpdate(typeof(TMessage),
                   _ =>
                   {
                       var mutators = new Collection<Handle<T>>();
                       mutators.Add((async (@event, state) => await handler(@event as TMessage, state)));
                       return mutators;
                   },
                   (_, existingHandler) =>
                   {
                       existingHandler.Add(async (@event, state) => await handler(@event as TMessage, state));
                       return existingHandler;
                   });


        protected virtual void When(Type messageType,
            Handle<T> handler) =>
           _handlers.AddOrUpdate(messageType,
                     _ =>
                     {
                         var mutators = new Collection<Handle<T>>();
                         mutators.Add(handler);
                         return mutators;
                     },
                     (_, existingHandler) =>
                     {
                         existingHandler.Add(handler);
                         return existingHandler;
                     });


        public virtual async Task HandleAsync<TMessage>(TMessage message,
            T connector)
        {
            _handlers.TryGetValue(message.GetType(), out var mutators);

            if (mutators == default)
                throw new ProjectionHandlerNotFoundException(message.GetType().Name);

            foreach (var mutator in mutators)
            {
                await mutator(message, connector).ConfigureAwait(false);
            }
        }
    }
}
