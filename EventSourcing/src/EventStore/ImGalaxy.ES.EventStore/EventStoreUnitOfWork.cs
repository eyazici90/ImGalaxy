using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using ImGalaxy.ES.Core;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{
    public class EventStoreUnitOfWork : IUnitOfWork
    {
        private readonly IChangeTracker _changeTracker;
        private readonly IEventStoreConnection _connection;
        private readonly IMediator _mediator;
        private readonly IEventSerializer _eventSerializer;
        private readonly IStreamNameProvider _streamNameProvider;
        public EventStoreUnitOfWork(IChangeTracker changeTracker, 
            IEventStoreConnection connection,
            IMediator mediator,
            IEventSerializer eventSerializer,
            IStreamNameProvider streamNameProvider)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider)); 
        }
       

        private async Task DispatchNotificationsAsync()
        {
            var notifications = this._changeTracker.GetChanges().Select(a => (a.Root as IAggregateChangeTracker));

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

        private async Task<IExecutionResult> AppendToStreamAsync()
        { 
            foreach (Aggregate aggregate in this._changeTracker.GetChanges())
            {
                EventData[] changes = (aggregate.Root as IAggregateChangeTracker).GetEvents()
                                               .Select(@event => new EventData(
                                                   Guid.NewGuid(),
                                                   @event.GetType().TypeQualifiedName(),
                                                   true,
                                                   Encoding.UTF8.GetBytes(this._eventSerializer.Serialize(@event)),
                                                   Encoding.UTF8.GetBytes(this._eventSerializer.Serialize(new EventMetadata
                                                   {
                                                       TimeStamp = DateTime.Now,
                                                       AggregateType = aggregate.Root.GetType().Name,
                                                       AggregateAssemblyQualifiedName = aggregate.Root.GetType().AssemblyQualifiedName,
                                                       IsSnapshot = false
                                                   }))
                                                   )).ToArray();
                try
                {
                    await this._connection.AppendToStreamAsync(_streamNameProvider.GetStreamName(aggregate.Root, aggregate.Identifier), aggregate.ExpectedVersion, changes);
                     
                }
                catch (WrongExpectedVersionException)
                {
                    throw;
                }
            }
            return ExecutionResult.Success;
        }

        public IExecutionResult SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<IExecutionResult> SaveChangesAsync()
        {
            await AppendToStreamAsync();
            await DispatchNotificationsAsync();
            _changeTracker.ResetChanges();

            return ExecutionResult.Success;
        } 
    }
}
