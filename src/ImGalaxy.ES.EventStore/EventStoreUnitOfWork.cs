using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Galaxy.Railway;
using ImGalaxy.ES.Core;
using MediatR;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class EventStoreUnitOfWork : IUnitOfWork
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IAggregateChangeTracker _changeTracker;
        private readonly IMediator _mediator;
        public EventStoreUnitOfWork(IAggregateStore aggregateStore,
            IAggregateChangeTracker changeTracker,
            IMediator mediator)
        {
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
                    await this._mediator.Publish(domainEvent).ConfigureAwait(false);
                });

            await Task.WhenAll(tasks);
        }

        private async Task<IExecutionResult> AppendChangesToStreamAsync()
        {
            foreach (Aggregate aggregate in this._changeTracker.GetChanges())
            {
                await _aggregateStore.Save(aggregate).ConfigureAwait(false);
            }
            return ExecutionResult.Success;
        }

        public IExecutionResult SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<IExecutionResult> SaveChangesAsync()
        {
            await AppendChangesToStreamAsync().ConfigureAwait(false);
            await DispatchNotificationsAsync().ConfigureAwait(false);
            _changeTracker.ResetChanges();

            return ExecutionResult.Success;
        }  
    }
}
