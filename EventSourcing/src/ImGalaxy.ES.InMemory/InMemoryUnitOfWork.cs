using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private readonly IChangeTracker _changeTracker;
        private readonly IInMemoryConnection _connection;
        public InMemoryUnitOfWork(IChangeTracker changeTracker,
            IInMemoryConnection connection)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        private async Task<IExecutionResult> AppendChangesToStreamAsync()
        {
            foreach (Aggregate aggregate in this._changeTracker.GetChanges())
            {
                await AppendToStreamAsync(aggregate);
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

        public async Task<IExecutionResult> AppendToStreamAsync(Aggregate aggregate)
        {
            InMemoryEventData[] changes = (aggregate.Root as IAggregateChangeTracker).GetEvents()
                                            .Select(@event => new InMemoryEventData(
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
                await this._connection.AppendToStreamAsync($"{aggregate.Root.GetType().Name}-{aggregate.Identifier}", aggregate.ExpectedVersion, changes);

            }
            catch (WrongExpectedStreamVersionException)
            {
                throw;
            }

            return ExecutionResult.Success;
        }
    }
}
