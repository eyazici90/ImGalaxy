using ImGalaxy.ES.Core;
using Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public class CommandActor<TState> : ReceiveActor<TState>
        where TState : class, IAggregateRootState<TState>, IAggregateRoot
    {   
        public IAggregateStore AggregateStore { get; }

        public string ActorId { get; private set; }

        public CommandActor(IAggregateStore aggregateStore)
        {
            AggregateStore = aggregateStore; 

            When<Started>(async ctx =>
                await RecoverStateAsync(ctx)
            ); 
        }

        private async Task RecoverStateAsync(IContext ctx)
        {
            ActorId = ctx.Self.Id;
            var state = await AggregateStore.Load<TState>(ctx.Self.Id);
            State = state.Root as TState;
        } 

        private async Task Apply(string identifier, AggregateRootState<TState>.Result result) =>
            await AppendToStreamAsync((int)ExpectedVersion.SafeStream, identifier, result);

        private async Task Apply((string, AggregateRootState<TState>.Result) result) =>
           await AppendToStreamAsync((int)ExpectedVersion.NoStream, result.Item1, result.Item2);

        private async Task AppendToStreamAsync(int aggregateVersion, string identifier, AggregateRootState<TState>.Result result)
        {
            await AggregateStore.Save(new Aggregate(identifier, aggregateVersion, result.State));

            (result.State as IAggregateChangeTracker).ClearEvents();
        }

        protected void When<TCommand>(Func<TCommand, string> identifyHandler, Func<TCommand, AggregateRootState<TState>.Result> handler)
           where TCommand : class
           => Handlers.Add(
               typeof(TCommand),
               ctx => Apply(identifyHandler(ctx.Message as TCommand), handler(ctx.Message as TCommand))
           );

        protected void When<TCommand>(Func<TCommand, (string, AggregateRootState<TState>.Result)> handler)
                   where TCommand : class
                   => Handlers.Add(
                       typeof(TCommand),
                        ctx => Apply(handler(ctx.Message as TCommand))
                   );
    }
}
