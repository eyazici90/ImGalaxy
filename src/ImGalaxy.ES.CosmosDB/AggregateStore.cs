using ImGalaxy.ES.Core;
using System;
using System.Threading.Tasks;
using Version = ImGalaxy.ES.Core.Version;

namespace ImGalaxy.ES.CosmosDB
{
    public class AggregateStore : AggregateStoreBase, IAggregateStore
    {
        private readonly IAggregateStoreDependencies _aggregateStoreDependencies;
        public AggregateStore(IAggregateStoreDependencies dependencies)
            : base(dependencies) =>
            _aggregateStoreDependencies = dependencies;

        public async Task<Aggregate> Load<T>(string id, int version = default) where T : class, IAggregateRootState<T>
        {
            var streamName = GetStreamNameOfRoot<T>(id);

            if (version == default) version = StreamPosition.Start;

            var slice = await ReadStreamEventsForwardAsync(streamName, version).ConfigureAwait(false);

            slice.ThrowsIf(s => !s.HasValue, new AggregateNotFoundException(streamName));

            T root = IntanceOfRoot<T>().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice.Value));

            ClearChangesOfRoot(root);

            return new Aggregate(id, slice.Value.Version, root as IAggregateRoot);
        }


        public async Task<IExecutionResult> Save<T>(string identifer, Version version, StateBase<T>.Result update) where T : class, IAggregateRootState<T>
        {
            CosmosEventData[] changes = update.Events.ToCosmosEventData(update.State.GetType());

            return await AppendToStreamInternalAsync(_aggregateStoreDependencies.StreamNameProvider.GetStreamName(update.State, identifer),
              version, changes).ConfigureAwait(false);
        }

        public async Task<IExecutionResult> Save(Aggregate aggregate)
        {
            CosmosEventData[] changes = (aggregate.Root as IAggregateRootChangeTracker)
                                                                            .GetEvents()
                                                                            .ToCosmosEventData(aggregate.Root.GetType());

            return await AppendToStreamInternalAsync(_aggregateStoreDependencies.StreamNameProvider.GetStreamName(aggregate.Root, aggregate.Identifier),
              aggregate.ExpectedVersion, changes).ConfigureAwait(false);
        }

        private async Task<IExecutionResult> AppendToStreamInternalAsync(string stream, Version expectedVersion, CosmosEventData[] events)
        {
            try
            {
                await _aggregateStoreDependencies.CosmosDBConnection
                    .AppendToStreamAsync(stream, expectedVersion, events).ConfigureAwait(false);
            }
            catch (WrongExpectedStreamVersionException)
            {
                throw ;
            }

            return ExecutionResult.Success; 
        }
    }
}
