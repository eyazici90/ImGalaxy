using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory
{
    public class WrongExpectedStreamVersionException : Exception
    {
        public WrongExpectedStreamVersionException()
        { }

        public WrongExpectedStreamVersionException(string expectedVersion, string streamVersion)
            : base($"expectedVersion is : {expectedVersion}, current stream version is :{streamVersion}")
        { }
    }
}
