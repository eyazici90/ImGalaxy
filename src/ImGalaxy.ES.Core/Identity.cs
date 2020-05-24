using System.Collections.Generic;

namespace ImGalaxy.ES.Core
{
    public abstract class Identity<T> : ValueObject
    {
        public T Id { get; }

        protected Identity(T id)
        {
            Id = id;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }
        public override string ToString() => Id.ToString(); 
    }
}
