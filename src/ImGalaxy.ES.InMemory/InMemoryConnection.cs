using System;
using System.Collections.Concurrent;
using System.Collections.Generic; 
using System.Threading;
using System.Threading.Tasks;
using Galaxy.Railway;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.InMemory
{
    public class InMemoryConnection : IInMemoryConnection
    {
        private readonly static SemaphoreSlim _locker = new SemaphoreSlim(1, 1);
        private readonly static ConcurrentDictionary<string, InMemoryStream> _inMemoryStore = new ConcurrentDictionary<string, InMemoryStream>();
        public async Task<IExecutionResult> AppendToStreamAsync(string streamId, long expectedVersion, params InMemoryEventData[] events)
        {
            await _locker.LockAsync(async () =>
            await AppendToStreamInternalAsync(streamId, expectedVersion, events)
        );
            return ExecutionResult.Success;
        }

        private async Task<IExecutionResult> AppendToStreamInternalAsync(string streamId, long expectedVersion, params InMemoryEventData[] events)
        {
            long eventPosition = 1;
            if (expectedVersion == ExpectedVersion.NoStream || expectedVersion == ExpectedVersion.Any)
            {
                await CreateNewStream(streamId, streamId, events);
                eventPosition++;
            }
            else
            {
                var existingStream = await this.ReadStreamEventsForwardAsync(streamId, 0,
                    int.MaxValue);

                existingStream.ThrowsIf(stream => !existingStream.HasValue, new AggregateNotFoundException(streamId))
                              .ThrowsIf(stream => expectedVersion != stream.Value.Version,
                                                  new WrongExpectedStreamVersionException(expectedVersion.ToString(),
                                                  existingStream.Value.Version.ToString()));

                expectedVersion = expectedVersion + events.Length;

                var newVersionedStream = existingStream.Value.ChangeVersion(expectedVersion);


                eventPosition = newVersionedStream.NextEventNumber;
            }

            foreach (var @event in events)
            {
                var newEvent = InMemoryEvent.Create(@event.EventId, streamId, eventPosition, @event.EventType, @event, @event.EventMetadata, DateTime.Now);
                InMemoryStream existingStream;

                _inMemoryStore.TryGetValue(streamId, out existingStream);

                var updatedStream = existingStream.AppendEvents(new List<InMemoryEvent> { newEvent });

                _inMemoryStore.TryUpdate(streamId, updatedStream, existingStream);

                eventPosition++;
            }

            return ExecutionResult.Success;
        }

        private async Task<IExecutionResult> CreateNewStream(string id, string streamType, params InMemoryEventData[] events)
        {
            var version = events.Length;

            var newStream = InMemoryStream.Create(id, streamType)
                                          .ChangeVersion(version);

            _inMemoryStore.TryAdd(id, newStream);

            return ExecutionResult.Success;
        }

        public async Task<Optional<InMemoryStream>> ReadStreamEventsBackwardAsync(string streamId, long start, int count)
        {
            return new Optional<InMemoryStream>(
                       _inMemoryStore[streamId]
                );
        }

        public async Task<Optional<InMemoryStream>> ReadStreamEventsForwardAsync(string streamId, long start, int count)
        {
            return new Optional<InMemoryStream>(
                     _inMemoryStore[streamId]
              );
        }
    }
}
