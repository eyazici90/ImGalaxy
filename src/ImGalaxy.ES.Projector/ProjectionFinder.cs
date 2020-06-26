using System.Collections.Generic;
using System.Linq;

namespace ImGalaxy.ES.Projector
{
    public static class ProjectionFinder
    {
        public static IEnumerable<IProjection<T>> FindProjections<T>(IEnumerable<object> projections,
            object @event)
        where T : class =>
           projections.Select(p => p as IProjection<T>)
                .Where(p => p.Handlers.TryGetValue(@event.GetType(), out var _));
    }
}
