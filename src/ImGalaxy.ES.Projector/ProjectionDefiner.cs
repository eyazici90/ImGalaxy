using System;
using System.Collections.Concurrent; 

namespace ImGalaxy.ES.Projector
{
    public static class ProjectionDefiner
    {
        private static readonly ConcurrentDictionary<Type, Type> _projectionMaps = new ConcurrentDictionary<Type, Type>();

        public static void SubscribeTo<T, TProjection>()
            where T : class
            where TProjection : class =>
            _projectionMaps.TryAdd(typeof(T), typeof(TProjection));

        public static Type From<T>() where T : class =>
            typeof(T);

        public static void To<TProjection>(this Type type) where TProjection : class =>
               _projectionMaps.TryAdd(type, typeof(TProjection));

        public static Type GetProjectionType(Type type)
        {
            _projectionMaps.TryGetValue(type, out var projection);

            return projection;
        }


    }
}
