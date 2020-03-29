using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public class AggregateCommandHandlerBase<T, TKey> where T : class, IAggregateRootState<T>
    {
        private readonly Func<string, int, Task<Aggregate>> _load;
        private readonly Func<Aggregate, Task<IExecutionResult>> _save;
        private AggregateCommandHandlerBase(Func<string, int, Task<Aggregate>> load,
            Func<Aggregate, Task<IExecutionResult>> save)
        {
            _load = load;
            _save = save;
        }


        public AggregateCommandHandlerBase(IAggregateStore aggregateStore)
            : this(async (id, version) => await aggregateStore.Load<T>(id, version),
                   async aggregate => await aggregateStore.Save(aggregate))
        {
        }

        protected virtual async Task<IExecutionResult> AddAsync(Func<Task<AggregateRootState<T>.Result>> factory, string id)
        {
            var result = await factory();

            return await _save(new Aggregate(id, (int)ExpectedVersion.NoStream, result.State as IAggregateRoot));
        }

        protected virtual async Task<IExecutionResult> UpdateAsync(TKey identifier,
            Func<T, Task> when,
            int version = default)
        {
            var aggregate = await _load(identifier.ToString(), version);

            await when(aggregate.Root as T);

            return await _save(aggregate);
        }
    }
}
