using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{ 
    public class OptimisticConcurrencyException : Exception
    {
        public OptimisticConcurrencyException()
        { }

        public OptimisticConcurrencyException(string message)
            : base(message)
        { }
    }
}
