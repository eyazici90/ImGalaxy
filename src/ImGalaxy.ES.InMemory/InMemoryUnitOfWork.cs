using Galaxy.Railway;
using ImGalaxy.ES.Core;
using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private readonly IAggregateChangeTracker _changeTracker;
        private readonly IAggregateStore _aggregateStore;
        public InMemoryUnitOfWork(IAggregateChangeTracker changeTracker,
            IAggregateStore aggregateStore)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        }
        private async Task<IExecutionResult> AppendChangesToStreamAsync()
        {
            foreach (Aggregate aggregate in this._changeTracker.GetChanges())
            {
                await _aggregateStore.Save(aggregate);
            }
            return ExecutionResult.Success;
        }

        public IExecutionResult SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<IExecutionResult> SaveChangesAsync()
        {
            await AppendChangesToStreamAsync();
            _changeTracker.ResetChanges();

            return ExecutionResult.Success;
        } 
        
    }
}
