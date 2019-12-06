using ImGalaxy.ES.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosDBUnitOfWork : IUnitOfWork
    {
        private readonly IChangeTracker _changeTracker;
        private readonly ICosmosDBConnection _cosmosDBConnection;
        private readonly IStreamNameProvider _streamNameProvider;
        public CosmosDBUnitOfWork(IChangeTracker changeTracker,
            ICosmosDBConnection cosmosDBConnection,
            IStreamNameProvider streamNameProvider)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _cosmosDBConnection = cosmosDBConnection ?? throw new ArgumentNullException(nameof(cosmosDBConnection));
            _streamNameProvider = streamNameProvider ?? throw new ArgumentNullException(nameof(streamNameProvider));
        }

        public IExecutionResult SaveChanges() => SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
                aggregate = ApplyVersionStrategy(aggregate);

                await this._cosmosDBConnection.AppendToStreamAsync(
                    _streamNameProvider.GetStreamName(aggregate.Root, aggregate.Identifier), aggregate.ExpectedVersion, changes);

            }
            catch (WrongExpectedStreamVersionException)
            {
                throw;
            }

            return ExecutionResult.Success;
        }

        private Aggregate ApplyVersionStrategy(Aggregate aggregate)
        {
            var result = VersionStrategy.IsAppliedByIVersion(aggregate.Root);
            var version = aggregate.ExpectedVersion;
            if (result)
                version = VersionStrategy.VersionOfRoot(aggregate.Root);

            return new Aggregate(aggregate.Identifier, version, aggregate.Root);
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
            _changeTracker.ResetChanges();
            return ExecutionResult.Success;
        }
    }
}
