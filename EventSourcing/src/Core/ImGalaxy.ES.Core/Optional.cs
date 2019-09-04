﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public struct Optional<T> : IEnumerable<T>, IEquatable<Optional<T>>
    {
        public static readonly Optional<T> Empty = new Optional<T>();

        private readonly bool _hasValue;
        private readonly T _value;

        public Optional(T value)
        {
            _value = value;
            _hasValue = value == null ? false : true;
        }

        public bool HasValue => _hasValue; 
        public T Value => !HasValue ? throw new InvalidOperationException() : _value; 

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
            {
                yield return _value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (GetType() != obj.GetType()) return false;
            return Equals((Optional<T>)obj);
        }

        public bool Equals(Optional<T> other)
        {
            if (_hasValue.Equals(other._hasValue))
            {
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                {
                    var enumerable1 = (IEnumerable)_value;
                    var enumerable2 = (IEnumerable)other._value;
                    if (enumerable1 == null && enumerable2 == null) return true;
                    if (enumerable1 == null || enumerable2 == null) return false;
                    var enumerator1 = enumerable1.GetEnumerator();
                    var enumerator2 = enumerable2.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        if (!(enumerator2.MoveNext() &&
                              EqualityComparer<object>.Default.Equals(enumerator1.Current, enumerator2.Current)))
                        {
                            return false;
                        }
                    }
                    return !enumerator2.MoveNext();
                }
                return EqualityComparer<T>.Default.Equals(_value, other._value);
            }
            return false;
        }

        public static bool operator ==(Optional<T> instance1, Optional<T> instance2)
        {
            return instance1.Equals(instance2);
        }

        public static bool operator !=(Optional<T> instance1, Optional<T> instance2)
        {
            return !instance1.Equals(instance2);
        }

        public override int GetHashCode()
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                var enumerable = (IEnumerable)_value;
                if (enumerable != null)
                {
                    var enumerator = enumerable.GetEnumerator();
                    var hashCode = _hasValue.GetHashCode();
                    while (enumerator.MoveNext())
                    {
                        hashCode ^= EqualityComparer<object>.Default.GetHashCode(enumerator.Current);
                    }
                    return hashCode ^ typeof(T).GetHashCode();
                }
            }
            return _hasValue.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(_value) ^ typeof(T).GetHashCode();
        }
    }
}