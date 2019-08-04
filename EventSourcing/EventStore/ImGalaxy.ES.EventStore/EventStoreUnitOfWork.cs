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
        private readonly ConcurrentDictionary<string, Aggregate> _aggregates;
        private readonly IEventStoreConnection _connection;
        private readonly IMediator _mediator;
        private readonly IEventSerializer _eventSerializer;
        private readonly IStreamNameProvider _streamNameProvider;
        public EventStoreUnitOfWork(IEventStoreConnection connection,
            IMediator mediator,
            IEventSerializer eventSerializer,
            IStreamNameProvider streamNameProvider)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
            _aggregates = new ConcurrentDictionary<string, Aggregate>();
        }
       
        public void Attach(Aggregate aggregate)
        {
            aggregate.ThrowsIfNull(new ArgumentNullException(nameof(aggregate)));

            aggregate.ThrowsIf(a=> !_aggregates.TryAdd(aggregate.Identifier, aggregate),
                new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                        $@"The aggregate of type '{aggregate.Root.GetType().Name}' with identifier '{aggregate.Identifier}' was already added.")));
           
        }
         
        public bool TryGet(string identifier, out Aggregate aggregate) => _aggregates.TryGetValue(identifier, out aggregate);
 
        public bool HasChanges() =>
             _aggregates.Values.Any(aggregate => aggregate.Root.HasChanges());
        
        public IEnumerable<Aggregate> GetChanges() =>
             _aggregates.Values.Where(aggregate => aggregate.Root.HasChanges());

        public async Task DispatchNotificationsAsync()
        {
            var notifications = this._aggregates.Values.Select(a => (a.Root as IAggregateChangeTracker));

            var domainEvents = notifications
                .SelectMany(x => x.GetChanges())
                .ToList();

            notifications.ToList()
                .ForEach(entity => entity.ClearChanges());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await this._mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }

        private async Task<int> AppendToStreamAsync()
        {
            int eventCount = 0;
            foreach (Aggregate aggregate in GetChanges())
            {
                EventData[] changes = (aggregate.Root as IAggregateChangeTracker).GetChanges()
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

                    eventCount = eventCount + changes.Length;
                }
                catch (WrongExpectedVersionException)
                {
                    throw;
                }
            }
            return eventCount;
        }

        public void SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<int> SaveChangesAsync()
        {
            var result = await AppendToStreamAsync();
            await DispatchNotificationsAsync();
            return result;
        }
    }
}
