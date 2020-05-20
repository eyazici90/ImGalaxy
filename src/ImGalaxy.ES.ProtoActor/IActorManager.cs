using Proto; 

namespace ImGalaxy.ES.ProtoActor
{
    public interface IActorManager : IActorRouter
    { 
        PID GetActor<TActor>() where TActor : IActor;
        PID GetActor<TActor>(string id) where TActor : IActor;
      
    }
}
