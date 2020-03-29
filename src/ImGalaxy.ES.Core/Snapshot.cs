namespace ImGalaxy.ES.Core
{
    public class Snapshot
    {
        public int Version { get; }

        public object State { get; }

        public Snapshot(int version, object state)
        {
            Version = version;
            State = state;
        }
    }
}
