using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class EventRecorder : IEventRecorder
    {
        private readonly List<object> _events;
        public EventRecorder() => _events = new List<object>();
        public IReadOnlyList<object> RecordedEvents => _events.AsReadOnly();

        public void Record(object @event) =>
            _events.Add(@event);
        
        public void Reset() =>
            _events?.Clear();
        
    }
}
