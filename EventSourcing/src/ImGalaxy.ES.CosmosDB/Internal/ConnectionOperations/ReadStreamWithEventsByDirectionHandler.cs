﻿using ImGalaxy.ES.Core;
using ImGalaxy.ES.CosmosDB.Documents;
using System;
using System.Collections.Generic;
using System.Text;
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

            var existingStream = await _getStreamDocumentByIdAsync(new GetStreamDocumentByIdAsync(id)); 

            if (!existingStream.HasValue)
                return Optional<CosmosStream>.Empty;

            var existingEvents = operation.EventFunc(id);

            var cosmosStream = existingStream.Value
                                             .ToCosmosStream(existingEvents);

            return new Optional<CosmosStream>(cosmosStream);
        }
    }
}
