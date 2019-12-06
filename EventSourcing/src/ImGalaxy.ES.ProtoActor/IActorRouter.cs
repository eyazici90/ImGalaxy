using Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.ProtoActor
{
    public interface IActorRouter
    {
        Task<T> RequestAsync<T>(PID actorId, object message);
        Task<T> RequestAsync<T, TActor>(string actorId, object message) where TActor : IActor;
        void Send(PID actorId, object message);
    }
}
