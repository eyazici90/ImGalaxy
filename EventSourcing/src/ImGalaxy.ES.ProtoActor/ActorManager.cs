using Proto;
using System;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public class ActorManager : IActorManager
    {
        public IRootContext RootContext => _lazyRootContext.Value;

        private static Lazy<IRootContext> _lazyRootContext = new Lazy<IRootContext>(() => new RootContext());

        private readonly IActorFactory _actorFactory;

        public ActorManager(IActorFactory actorFactory) =>
            _actorFactory = actorFactory;

        public PID GetActor<TActor>() where TActor : IActor =>
            _actorFactory.GetActor<TActor>();

        public PID GetActor<TActor>(string id) where TActor : IActor =>
            _actorFactory.GetActor<TActor>(id);

        public async Task<T> RequestAsync<T, TActor>(string actorId, object message)
            where TActor : IActor
        {
            var actorPId = GetActor<TActor>(actorId);

            return await RootContext.RequestAsync<T>(actorPId, message);
        }

        public async Task<T> RequestAsync<T>(PID actorId, object message) =>
            await RootContext.RequestAsync<T>(actorId, message);

        public void Send(PID actorId, object message) =>
            RootContext.Send(actorId, message);

    }
}
