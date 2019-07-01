using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class Snapshot
    {
        public int Version { get; private set; }

        public object State { get; private set; }

        public Snapshot(int version, object state)
        {
            Version = version;
            State = state;
        }
    }
}
