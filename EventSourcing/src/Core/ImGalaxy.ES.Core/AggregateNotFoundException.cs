using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException()
        { }

        public AggregateNotFoundException(string message)
            : base(message)
        { }

        public AggregateNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
