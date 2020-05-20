using Proto;
using System;

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
                catch (Exception ex)
                {
                    var errorMsg = new ExceptionOccuredDuringHandleEvent(ex);

                    await next(context, envelope.WithMessage(errorMsg));
                }
            }; 
    }
}
