using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using ImGalaxy.ES.Core;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StreamPosition = ImGalaxy.ES.Core.StreamPosition;
using Version = ImGalaxy.ES.Core.Version;

namespace ImGalaxy.ES.EventStore
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

            if (version == default)
                version = StreamPosition.Start; 

            StreamEventsSlice slice = await ReadStreamEventsForwardAsync(streamName, version);

            slice.ThrowsIf(s => s.Status == SliceReadStatus.StreamDeleted || s.Status == SliceReadStatus.StreamNotFound,
                new AggregateNotFoundException(streamName));

            T root = IntanceOfRoot<T>().Value;

            ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));

            while (!slice.IsEndOfStream)
            {
                slice = await ReadStreamEventsForwardAsync(streamName, slice.NextEventNumber);

                ApplyChangesToRoot(root, DeserializeEventsFromSlice(slice));
            }

            ClearChangesOfRoot(root);

            return new Aggregate(id, slice.LastEventNumber, root as IAggregateRoot);
        }


        public async Task<IExecutionResult> Save<T>(string identifer, Version version, AggregateRootState<T>.Result update) where T : class, IAggregateRootState<T>
        {
            EventData[] changes = update.Events
                                          .Select(@event => new EventData(
                                              Guid.NewGuid(),
                                              @event.GetType().TypeQualifiedName(),
                                              true,
                                              Encoding.UTF8.GetBytes(this._aggregateStoreDependencies.EventSerializer.Serialize(@event)),
                                              Encoding.UTF8.GetBytes(this._aggregateStoreDependencies.EventSerializer.Serialize(new EventMetadata
                                              {
                                                  TimeStamp = DateTime.Now,
                                                  AggregateType = update.State.GetType().Name,
                                                  AggregateAssemblyQualifiedName = update.State.GetType().AssemblyQualifiedName,
                                                  IsSnapshot = false
                                              }))
                                              )).ToArray();

            return await AppendToStreamInternalAsync(_aggregateStoreDependencies.StreamNameProvider.GetStreamName(update.State, identifer),
                version, changes);
        }

        public async Task<IExecutionResult> Save(Aggregate aggregate)
        {
            EventData[] changes = (aggregate.Root as IAggregateRootChangeTracker).GetEvents()
                                         .Select(@event => new EventData(
                                             Guid.NewGuid(),
                                             @event.GetType().TypeQualifiedName(),
                                             true,
                                             Encoding.UTF8.GetBytes(_aggregateStoreDependencies.EventSerializer.Serialize(@event)),
                                             Encoding.UTF8.GetBytes(_aggregateStoreDependencies.EventSerializer.Serialize(new EventMetadata
                                             {
                                                 TimeStamp = DateTime.Now,
                                                 AggregateType = aggregate.Root.GetType().Name,
                                                 AggregateAssemblyQualifiedName = aggregate.Root.GetType().AssemblyQualifiedName,
                                                 IsSnapshot = false
                                             }))
                                             )).ToArray();

            return await AppendToStreamInternalAsync(_aggregateStoreDependencies.StreamNameProvider.GetStreamName(aggregate.Root, aggregate.Identifier),
                aggregate.ExpectedVersion, changes);
        }

        private async Task<IExecutionResult> AppendToStreamInternalAsync(string stream, long expectedVersion, EventData[] events)
        {
            try
            {
                await this._aggregateStoreDependencies.EventStoreConnection.AppendToStreamAsync(stream, expectedVersion, events);

            }
            catch (WrongExpectedVersionException)
            {
                throw;
            }

            return ExecutionResult.Success;

        }
    }
}
