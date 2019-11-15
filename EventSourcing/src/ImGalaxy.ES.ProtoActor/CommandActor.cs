using ImGalaxy.ES.Core;
using Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public class CommandActor<TState> : IActor
        where TState : class, IAggregateRoot
    {
        public TState State { get; set; }
        private readonly IAggregateRootRepository<TState> _aggregateRootRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CommandActor(IAggregateRootRepository<TState> aggregateRootRepository,
            IUnitOfWork unitOfWork)
        {
            _aggregateRootRepository = aggregateRootRepository;

            _unitOfWork = unitOfWork;

            When<Started>(async ctx =>
                await RecoverStateAsync(ctx)
            ); 
        }

        private async Task RecoverStateAsync(IContext ctx)
        {
            var state = await _aggregateRootRepository.GetAsync(ctx.Self.Id);
            State = state.HasValue ? state.Value
                                   : (TState)Activator.CreateInstance(typeof(TState), true);
        }

        private readonly Dictionary<Type, Func<IContext, Task>> _handlers =
             new Dictionary<Type, Func<IContext, Task>>();

        public async Task ReceiveAsync(IContext context)
        {
            if (!_handlers.TryGetValue(context.Message.GetType(), out var handler))
                return;
            try
            {
                await handler(context);
            }
            catch (Exception ex)
            {
                var whatHappned = new ExceptionOccuredDuringHandleEvent(ex);
            }

            if (context.Sender != null)
                context.Respond(State);
        }

        private async Task Apply(string identifier, AggregateRootState<TState>.Result result) =>
            await AppendToStreamAsync(2, identifier, result);

        private async Task Apply((string, AggregateRootState<TState>.Result) result) =>
           await AppendToStreamAsync(-1, result.Item1, result.Item2);

        private async Task AppendToStreamAsync(int aggregateVersion, string identifier, AggregateRootState<TState>.Result result) 
        {
            await _unitOfWork.AppendToStreamAsync(new Aggregate(identifier, aggregateVersion, result.State));

            (result.State as IAggregateChangeTracker).ClearEvents();
        }

        protected void When<TCommand>(Func<IContext, Task> handler)
              where TCommand : class
              => _handlers.Add(typeof(TCommand), handler);

        //protected void When<TCommand>(Func<TCommand, Task> handler)
        //    where TCommand : class
        //    => _handlers.Add(
        //        typeof(TCommand),
        //        ctx => handler(ctx.Message as TCommand));

        protected void When<TCommand>(Func<TCommand, string> identifyHandler, Func<TCommand, AggregateRootState<TState>.Result> handler)
            where TCommand : class
            => _handlers.Add(
                typeof(TCommand),
                ctx => Apply(identifyHandler(ctx.Message as TCommand), handler(ctx.Message as TCommand))
            );
        protected void When<TCommand>(Func<TCommand, (string, AggregateRootState<TState>.Result)> handler)
                   where TCommand : class
                   => _handlers.Add(
                       typeof(TCommand),
                        ctx => Apply(handler(ctx.Message as TCommand))
                   );
    }
}
