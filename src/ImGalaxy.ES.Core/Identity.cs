using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public abstract class Identity<T> : IEquatable<Identity<T>>
    {
        public T Id { get; }

        protected Identity(T id)
        {
            Id = id;
        }
        public override string ToString() => Id.ToString();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Identity<T>)obj);
        }

        public bool Equals(Identity<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Id);
        }

        public static bool operator ==(Identity<T> left, Identity<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Identity<T> left, Identity<T> right)
        {
            return !Equals(left, right);
        }
    }
}
