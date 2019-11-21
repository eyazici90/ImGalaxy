using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class Identity<T> 
    {
        public T Id { get; }
        public Identity(T id)
        {
            Id = id;
        }
        public override string ToString() => Id.ToString();
    }
}
