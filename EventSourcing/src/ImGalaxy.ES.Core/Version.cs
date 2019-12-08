using System;
using System.Collections.Generic;
using System.Text;

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

        public void WithMetaData(string metaData) =>
            new Version(this.Value, metaData);

        public static implicit operator long(Version self) => self.Value;
    }
}
