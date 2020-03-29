using System; 
using System.Threading.Tasks;
using static ImGalaxy.ES.Projector.ProjectorDelegates;

namespace ImGalaxy.ES.Projector
{
    public abstract class Projector : IProjector
    {
        private readonly Get _get;
        private readonly UpdateOrInsert _updateOrInsert;
        private readonly FindProjection _findProjection;
        public Projector(Get get,
            UpdateOrInsert updateOrInsert,
            FindProjection findProjection)
        {
            _get = get;
            _updateOrInsert = updateOrInsert;
            _findProjection = findProjection;
        }

        public async Task ProjectAsync<T>(object @event) where T : class
        {
            var projection = _findProjection(typeof(T));

            if (projection == default)
                throw new ArgumentNullException($"Projection could not be found {typeof(T).Name}");

            await (projection as IProjection<T>).HandleAsync(_updateOrInsert,
                @event,
                (T)Activator.CreateInstance(typeof(T), true),
                @event.GetType());
        }

        public async Task ProjectAsync<T>(string id,
            object @event) where T : class
        {
            T state = await _get(id);

            var projection = _findProjection(typeof(T));

            if (projection == default)
                throw new ArgumentNullException($"Projection could not be found {typeof(T).Name}");

            await (projection as IProjection<T>).HandleAsync(_updateOrInsert,
                @event,
                state,
                @event.GetType());
        }
    }
}
