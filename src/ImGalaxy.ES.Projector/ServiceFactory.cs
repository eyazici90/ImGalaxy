using System;
using System.Collections.Generic;

namespace ImGalaxy.ES.Projector
{
    public delegate IEnumerable<object> ServiceFactory(Type serviceType);
}
