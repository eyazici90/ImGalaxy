using EventStore.ClientAPI;
using Galaxy.Railway;
using ImGalaxy.ES.Core;
using System; 
using System.Text;
using System.Threading.Tasks;
using StreamPosition = ImGalaxy.ES.Core.StreamPosition;

namespace ImGalaxy.ES.EventStore
{
    public class SnapshotReaderEventStore: ISnapshotReader
    {
        private readonly IEventStoreConnection _connection;
        private readonly IEventDeserializer _deserializer;
        public SnapshotReaderEventStore(IEventStoreConnection connection,
            IEventDeserializer deserializer)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }
        public async Task<Optional<Snapshot>> GetLastSnapshot(string snapshotStream)
        {
            snapshotStream.ThrowsIfNull(new ArgumentNullException(nameof(snapshotStream)));

            var slice = await _connection.ReadStreamEventsBackwardAsync(snapshotStream, StreamPosition.End, 1, false);

            if (CheckIfStreamIsNotFound(slice)) return Optional<Snapshot>.Empty;

            var e = slice.Events[0].Event;
            var eData = this._deserializer.Deserialize(Type.GetType(e.EventType, true)
                                                        , Encoding.UTF8.GetString(e.Data));

            var eMetaData = this._deserializer.Deserialize<EventMetadata>(Encoding.UTF8.GetString(e.Metadata));

            return new Optional<Snapshot>(new Snapshot(eMetaData.Version, eData));
        }
        private bool CheckIfStreamIsNotFound(StreamEventsSlice slice) => slice.Status == SliceReadStatus.StreamDeleted 
                                || slice.Status == SliceReadStatus.StreamNotFound 
                                || (slice.Events.Length == 0 && slice.NextEventNumber == -1);

    }
}
