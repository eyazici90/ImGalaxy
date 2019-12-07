using ImGalaxy.ES.Core;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBUnitOfWork : IUnitOfWork
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IChangeTracker _changeTracker;
        public CosmosDBUnitOfWork(IChangeTracker changeTracker,
            IAggregateStore aggregateStore)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore)); 
        }

        public IExecutionResult SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task<IExecutionResult> AppendChangesToStreamAsync()
        {
            foreach (Aggregate aggregate in this._changeTracker.GetChanges())
            {
                await _aggregateStore.Save(aggregate);
            }
            return ExecutionResult.Success;
        }

        public async Task<IExecutionResult> SaveChangesAsync()
        {
            await AppendChangesToStreamAsync();
            _changeTracker.ResetChanges();
            return ExecutionResult.Success;
        }
    }
}
