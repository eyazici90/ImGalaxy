using ImGalaxy.ES.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Version = ImGalaxy.ES.Core.Version;

namespace ImGalaxy.ES.CosmosDB
{
    public class AggregateStore :  AggregateStoreBase, IAggregateStore
    {
        private readonly IAggregateStoreDependencies _aggregateStoreDependencies;
        public AggregateStore(IAggregateStoreDependencies dependencies)
            : base(dependencies) =>
            _aggregateStoreDependencies = dependencies;


        public async Task<Aggregate> Load<T>(string id, int version = default) where T : class, IAggregateRootState<T>
        {
            var streamName = GetStreamNameOfRoot<T>(id);

            if (version == default)
                version = StreamPosition.Start;

            var slice = await ReadStreamEventsForwardAsync(streamName, version);

            slice.ThrowsIf(s => !s.HasValue,
                  new AggregateNotFoundException(streamName));

            T root = IntanceOfRoot<T>().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice.Value));

            ClearChangesOfRoot(root);

            return new Aggregate(id, slice.Value.Version, root as IAggregateRoot);
        }


        public async Task<IExecutionResult> Save<T>(string identifer, Version version, StateBase<T>.Result update) where T : class, IAggregateRootState<T>
        {
            CosmosEventData[] changes = update.Events
                                            .Select(@event => new CosmosEventData(
                                                Guid.NewGuid().ToString(),
                                                @event.GetType().TypeQualifiedName(),
                                                @event,
                                                   new EventMetadata
                                                   {
                                                       TimeStamp = DateTime.Now,
                                                       AggregateType = update.State.GetType().Name,
                                                       AggregateAssemblyQualifiedName = update.State.GetType().AssemblyQualifiedName,
                                                       IsSnapshot = false
                                                   }
                                                )).ToArray();

            return await AppendToStreamInternalAsync(_aggregateStoreDependencies.StreamNameProvider.GetStreamName(update.State, identifer),
              version, changes);
        }

        public async Task<IExecutionResult> Save(Aggregate aggregate)
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

            return await AppendToStreamInternalAsync(_aggregateStoreDependencies.StreamNameProvider.GetStreamName(aggregate.Root, aggregate.Identifier),
              aggregate.ExpectedVersion, changes);
        }

        private async Task<IExecutionResult> AppendToStreamInternalAsync(string stream, Version expectedVersion, CosmosEventData[] events)
        {
            try
            {

                await this._aggregateStoreDependencies.CosmosDBConnection.AppendToStreamAsync(
                    stream, expectedVersion, events);

            }
            catch (WrongExpectedStreamVersionException)
            {
                throw;
            }

            return ExecutionResult.Success;

        }
    }
}
