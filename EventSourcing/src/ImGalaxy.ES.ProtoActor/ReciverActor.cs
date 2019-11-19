using ImGalaxy.ES.Core;
using Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public abstract class ReciverActor<TState> where TState : class, IAggregateRoot
    {
        protected readonly Dictionary<Type, Func<IContext, Task>> Handlers =  new Dictionary<Type, Func<IContext, Task>>();
        public TState State { get; set; }

        public async Task ReceiveAsync(IContext context)
        {
            if (!Handlers.TryGetValue(context.Message.GetType(), out var handler))
                return;

            await handler(context);

            if (context.Sender != null)
                context.Respond(State);
        }

        protected void When<TMessage>(Func<IContext, Task> handler)
         where TMessage : class
         => Handlers.Add(typeof(TMessage), handler);
         

    }
}
