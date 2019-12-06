using Proto;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public interface IActorManager : IActorRouter
    {
        IRootContext RootContext { get; }
        PID GetActor<TActor>() where TActor : IActor;
        PID GetActor<TActor>(string id) where TActor : IActor;
      
    }
}
