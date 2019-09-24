using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class Aggregate
    {
        public string Identifier { get; }

        public int ExpectedVersion { get; }

        public string RootType { get; }

        public IAggregateRoot Root { get; }

        public Aggregate(string identifier, int expectedVersion, IAggregateRoot root)
        {
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            this.ExpectedVersion = expectedVersion;
            this.Root = root ?? throw new ArgumentNullException(nameof(root));
            this.RootType = root.GetType().Name;
        }
    }
}
