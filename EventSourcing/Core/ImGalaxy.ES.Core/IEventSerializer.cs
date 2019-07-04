using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IEventSerializer
    {
        string Serialize(object obj, bool camelCase = true, bool indented = false);
    }
}
