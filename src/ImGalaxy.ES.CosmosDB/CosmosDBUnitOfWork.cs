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
        private readonly IAggregateChangeTracker _changeTracker;
        private readonly IMediator _mediator;
        public CosmosDBUnitOfWork(IAggregateChangeTracker changeTracker,
            IAggregateStore aggregateStore,
            IMediator mediator)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

        private async Task DispatchNotificationsAsync()
        {
            var notifications = this._changeTracker.GetChanges().Select(a => (a.Root as IAggregateRootChangeTracker));

            var domainEvents = notifications
                .SelectMany(x => x.GetEvents())
                .ToList();

            notifications.ToList()
                .ForEach(entity => entity.ClearEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await this._mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }

        public async Task<IExecutionResult> SaveChangesAsync()
        {
            await AppendChangesToStreamAsync();
            await DispatchNotificationsAsync();
            _changeTracker.ResetChanges();
            return ExecutionResult.Success;
        }
    }
}
