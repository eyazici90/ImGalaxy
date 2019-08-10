using ImGalaxy.ES.Core;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBUnitOfWork : IUnitOfWork
    {
        private readonly ICosmosDBConnection _cosmosDBConnection;
        private readonly IMediator _mediator; 
        private readonly IStreamNameProvider _streamNameProvider;
        private readonly ConcurrentDictionary<string, Aggregate> _aggregates;
        public CosmosDBUnitOfWork(ICosmosDBConnection cosmosDBConnection, 
            IMediator mediator,
            IStreamNameProvider streamNameProvider)
        {
            _cosmosDBConnection = cosmosDBConnection ?? throw new ArgumentNullException(nameof(cosmosDBConnection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); 
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
            _aggregates = new ConcurrentDictionary<string, Aggregate>();
        }

        public void Attach(Aggregate aggregate)
        {
            aggregate.ThrowsIfNull(new ArgumentNullException(nameof(aggregate)));

            aggregate.ThrowsIf(a => !_aggregates.TryAdd(aggregate.Identifier, aggregate),
                new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                        $@"The aggregate of type '{aggregate.Root.GetType().Name}' with identifier '{aggregate.Identifier}' was already added.")));
        }

        public bool TryGet(string identifier, out Aggregate aggregate) => _aggregates.TryGetValue(identifier, out aggregate);

        public bool HasChanges() =>
             _aggregates.Values.Any(aggregate => aggregate.Root.HasChanges());

        public IEnumerable<Aggregate> GetChanges() =>
             _aggregates.Values.Where(aggregate => aggregate.Root.HasChanges());

        public void SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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

        private async Task AppendToStreamAsync()
        { 
            foreach (Aggregate aggregate in GetChanges())
            {
                CosmosEventData[] changes = (aggregate.Root as IAggregateChangeTracker).GetChanges()
                                               .Select(@event => new CosmosEventData(
                                                   Guid.NewGuid().ToString(),
                                                   @event.GetType().TypeQualifiedName(),
                                                   @event,
                                                      new EventMetadata
                                                       {
                                                           TimeStamp = DateTime.Now,
                                                           AggregateType = aggregate.Root.GetType().Name,
                                                           AggregateAssemblyQualifiedName = aggregate.Root.GetType().AssemblyQualifiedName,
                                                           IsSnapshot = false
                                                       }
                                                   )).ToArray();
                try
                {
                    await this._cosmosDBConnection.AppendToStreamAsync(_streamNameProvider.GetStreamName(aggregate.Root, aggregate.Identifier), aggregate.ExpectedVersion, changes);

                }
                catch (WrongExpectedVersionException)
                {
                    throw;
                }
            }
        }
        public async Task SaveChangesAsync()
        {
            await AppendToStreamAsync();
            await DispatchNotificationsAsync();
            DetachAllAggregates();
        }
        private void DetachAllAggregates() =>
            _aggregates.Clear();
    }
}
