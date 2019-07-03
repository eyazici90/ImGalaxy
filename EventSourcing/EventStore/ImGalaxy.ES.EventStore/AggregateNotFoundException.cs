﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
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
