using Proto;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public interface IActorManager
    {
        PID GetActor<TActor>() where TActor : IActor;
        PID GetActor<TActor>(string id) where TActor : IActor;
        Task<T> RequestAsync<T>(PID actorId, object message);
        Task<T> RequestAsync<T, TActor>(string actorId, object message) where TActor : IActor;  
        void Send(PID actorId, object message);
    }
}
