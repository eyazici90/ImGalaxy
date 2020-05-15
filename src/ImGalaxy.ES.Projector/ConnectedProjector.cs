using System;
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
            var projection = _serviceFactory(typeof(IProjection<T>));

            if (projection == default)
                throw new ProjectionNotFoundException(typeof(T).Name);

            await ((IProjection<T>)projection).HandleAsync(@event, _connector).ConfigureAwait(false);
        }
    }
}
