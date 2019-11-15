using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.ProtoActor
{
    public class ExceptionOccuredDuringHandleEvent
    {
        public Exception Exception { get; }
        public ExceptionOccuredDuringHandleEvent(Exception exception)
        {
            Exception = exception;
        }
    }
}
