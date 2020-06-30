using Galaxy.Railway;
using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = ImGalaxy.ES.Core.Version;

namespace ImGalaxy.ES.InMemory
{
    public class AggregateStore : AggregateStoreBase, IAggregateStore
    {
        private readonly IInMemoryConnection _connection;
        public AggregateStore(IInMemoryConnection connection) : base(connection) =>
            _connection = connection;

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

            return new Aggregate(id, slice.Value.LastEventNumber, root as IAggregateRoot);
        }

        public async Task<IExecutionResult> Save(Aggregate aggregate)
        {
            InMemoryEventData[] changes = (aggregate.Root as IAggregateRootChangeTracker).GetEvents()
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

            return await AppendToStreamInternalAsync($"{aggregate.Root.GetType().Name}-{aggregate.Identifier}", aggregate.ExpectedVersion, changes);
        }


        public async Task<IExecutionResult> Save<T>(string identifer, Version version, StateBase<T>.Result update) where T : class, IAggregateRootState<T>
        {
            InMemoryEventData[] changes = update.Events
                                           .Select(@event => new InMemoryEventData(
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

            return await AppendToStreamInternalAsync($"{update.State.GetType().Name}-{identifer}", version, changes);
        }

        private async Task<IExecutionResult> AppendToStreamInternalAsync(string stream, long expectedVersion, InMemoryEventData[] events)
        {
            try
            {
                await this._connection.AppendToStreamAsync(stream, expectedVersion, events);

            }
            catch (WrongExpectedStreamVersionException)
            {
                throw;
            }

            return ExecutionResult.Success;

        }
    }
}
