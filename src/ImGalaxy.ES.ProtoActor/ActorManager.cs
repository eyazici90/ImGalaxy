using Proto;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public class ActorManager : IActorManager
    {
        private readonly IActorFactory _actorFactory;
        private readonly ActorSystem _actorSystem;
        public ActorManager(IActorFactory actorFactory,
            ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
            _actorFactory = actorFactory;
        }

        public PID GetActor<TActor>() where TActor : IActor =>
            _actorFactory.GetActor<TActor>();

        public PID GetActor<TActor>(string id) where TActor : IActor =>
            _actorFactory.GetActor<TActor>(id);

        public async Task<T> RequestAsync<T, TActor>(string actorId, object message)
            where TActor : IActor
        {
            var actorPId = GetActor<TActor>(actorId);

            return await _actorSystem.Root.RequestAsync<T>(actorPId, message);
        }

        public async Task<T> RequestAsync<T>(PID actorId, object message) =>
            await _actorSystem.Root.RequestAsync<T>(actorId, message);

        public void Send(PID actorId, object message) =>
            _actorSystem.Root.Send(actorId, message);

    }
}
