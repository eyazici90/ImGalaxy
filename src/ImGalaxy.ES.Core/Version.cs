namespace ImGalaxy.ES.Core
{
    public class Version
    {
        public long Value { get; }
        public string MetaData { get; }

        private Version(long value,
            string metaData)
        {
            Value = value;
            MetaData = metaData;
        }

        public Version(long value) : this(value, default)
        {
        }

        public Version WithMetaData(string metaData) =>
            new Version(Value, metaData);

        public Version WithVersion(long version) =>
            new Version(version, MetaData);

        public static implicit operator long(Version self) => self.Value;

        public static implicit operator Version(long value) => new Version(value);
    }
}
