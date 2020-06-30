using Galaxy.Railway; 
using ImGalaxy.ES.CosmosDB.Documents;
using System; 
using System.Threading;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class ReadStreamWithEventsByDirectionHandler : IOperationHandler<ReadStreamWithEventsByDirection, Optional<CosmosStream>>
    {
        private readonly Func<GetStreamDocumentByIdAsync, Task<Optional<StreamDocument>>> _getStreamDocumentByIdAsync;
        public ReadStreamWithEventsByDirectionHandler(Func<GetStreamDocumentByIdAsync, Task<Optional<StreamDocument>>> getStreamDocumentByIdAsync) =>
            _getStreamDocumentByIdAsync = getStreamDocumentByIdAsync;

        async Task<Optional<CosmosStream>> IOperationHandler<ReadStreamWithEventsByDirection, Optional<CosmosStream>>.Handle(ReadStreamWithEventsByDirection operation, CancellationToken cancellationToken)
        {
            var id = CosmosStreamNameStrategy.GetStreamIdentifier(operation.StreamId);

            var existingStream = await _getStreamDocumentByIdAsync(new GetStreamDocumentByIdAsync(id)).ConfigureAwait(false);

            if (!existingStream.HasValue)
                return Optional<CosmosStream>.Empty;

            var existingEvents = await operation.EventFunc(id).ConfigureAwait(false);

            var cosmosStream = existingStream.Value.ToCosmosStream(existingEvents);

            return new Optional<CosmosStream>(cosmosStream);
        }
    }
}
