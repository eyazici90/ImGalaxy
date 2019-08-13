using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{ 
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException()
        { }

        public WrongExpectedVersionException(string message)
            : base(message)
        { }
    }
}
