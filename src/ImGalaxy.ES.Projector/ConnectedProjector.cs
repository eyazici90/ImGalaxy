using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public class ConnectedProjector<T> : IProjector
     where T : class
    {
        private readonly T _connector;
        private readonly ServiceFactory _serviceFactory;
        public ConnectedProjector(T connector,
            ServiceFactory serviceFactory)
        {
            _connector = connector ?? throw new ArgumentNullException(nameof(connector));
            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task ProjectAsync(object @event)
        {
            var projections = _serviceFactory(typeof(IProjection<T>));

            if (projections == default) throw new ProjectionNotRegisteredException(typeof(T).Name);

            var foundProjections = ProjectionFinder.FindProjections<T>(projections, @event);

            if (!foundProjections.Any()) throw new ProjectionNotFoundException(typeof(T).Name);

            foreach (var projection in foundProjections)
            {
                await projection.HandleAsync(@event, _connector).ConfigureAwait(false);
            }
        }
    }
}
