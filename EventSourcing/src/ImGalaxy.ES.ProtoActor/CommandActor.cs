using ImGalaxy.ES.Core;
using Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public class CommandActor<TState> : ReceiveActor<TState>
        where TState : class, IAggregateRoot
    { 
        public IAggregateRootRepository<TState> AggregateRootRepository { get; }
        public IUnitOfWork UnitOfWork { get; }

        public CommandActor(IAggregateRootRepository<TState> aggregateRootRepository,
            IUnitOfWork unitOfWork)
        {
            AggregateRootRepository = aggregateRootRepository;

            UnitOfWork = unitOfWork;

            When<Started>(async ctx =>
                await RecoverStateAsync(ctx)
            );

            When<ExceptionOccuredDuringHandleEvent>(async ctx => 
            {
                if (ctx.Sender != null)
                    ctx.Respond(ctx.Message);
            });
        }

        private async Task RecoverStateAsync(IContext ctx)
        {
            var state = await AggregateRootRepository.GetAsync(ctx.Self.Id);
            State = state.HasValue ? state.Value
                                   : (TState)Activator.CreateInstance(typeof(TState), true);
        } 

        private async Task Apply(string identifier, AggregateRootState<TState>.Result result) =>
            await AppendToStreamAsync(2, identifier, result);

        private async Task Apply((string, AggregateRootState<TState>.Result) result) =>
           await AppendToStreamAsync(-1, result.Item1, result.Item2);

        private async Task AppendToStreamAsync(int aggregateVersion, string identifier, AggregateRootState<TState>.Result result)
        {
            await UnitOfWork.AppendToStreamAsync(new Aggregate(identifier, aggregateVersion, result.State));

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
