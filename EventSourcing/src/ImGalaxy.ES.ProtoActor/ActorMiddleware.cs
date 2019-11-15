using Proto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.ProtoActor
{
    public static class ActorMiddleware
    {
        public static Receiver Exception(Receiver next, string actorType) =>
            async (context, envelope) =>
            { 
                try
                {
                    await next(context, envelope);
                }
                catch (Exception)
                {
                    throw;
                }
            };
    }
}
