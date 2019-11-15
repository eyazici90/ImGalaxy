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
        private readonly IChangeTracker _changeTracker;
        private readonly ICosmosDBConnection _cosmosDBConnection;
        private readonly IMediator _mediator;
        private readonly IStreamNameProvider _streamNameProvider;
        public CosmosDBUnitOfWork(IChangeTracker changeTracker,
            ICosmosDBConnection cosmosDBConnection,
            IMediator mediator,
            IStreamNameProvider streamNameProvider)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _cosmosDBConnection = cosmosDBConnection ?? throw new ArgumentNullException(nameof(cosmosDBConnection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
        }
         
        public IExecutionResult SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task DispatchNotificationsAsync()
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

        public async Task<IExecutionResult> AppendToStreamAsync(Aggregate aggregate)
        { 
                CosmosEventData[] changes = (aggregate.Root as IAggregateChangeTracker).GetEvents()
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
                catch (WrongExpectedStreamVersionException)
                {
                    throw;
                }
            
            return ExecutionResult.Success;
        }

        private async Task<IExecutionResult> AppendChangesToStreamAsync()
        {
            foreach (Aggregate aggregate in this._changeTracker.GetChanges())
            {
                await AppendToStreamAsync(aggregate);
            }
            return ExecutionResult.Success;
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
