﻿using System; 

namespace ImGalaxy.ES.CosmosDB
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
