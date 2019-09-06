using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.InMemory
{
    public static class ExpectedVersion
    {
        public const long Any = -2;
        public const long NoStream = -1;
        public const long EmptyStream = -1;
        public const long StreamExists = -4;
        public const long New = 1;
    }
}
