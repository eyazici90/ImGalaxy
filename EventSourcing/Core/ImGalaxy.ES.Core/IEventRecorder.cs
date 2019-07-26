using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IEventRecorder
    {
        IReadOnlyList<object> RecordedEvents { get; }
        void Record(object @event);
        void Reset();
    }
}
