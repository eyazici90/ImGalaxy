using System; 
using System.Threading;
using System.Threading.Tasks;
using Galaxy.Railway; 
using ImGalaxy.ES.CosmosDB.Documents;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class CreateNewStreamHandler : IOperationHandler<CreateNewStream>
    { 
        private readonly Func<StreamDocument, Task> _createItemAsyc;
        internal CreateNewStreamHandler(Func<StreamDocument, Task> createItemAsyc) =>
            _createItemAsyc = createItemAsyc;
         
        async Task<IExecutionResult> IOperationHandler<CreateNewStream, IExecutionResult>.Handle(CreateNewStream operation, CancellationToken cancellationToken)
        {
            var version = operation.Events.Length;

            var newStream = CosmosStream.Create(operation.Id, operation.StreamType)
                                          .ChangeVersion(version); 

            await _createItemAsyc(newStream.ToCosmosStreamDocument()).ConfigureAwait(false);

            return ExecutionResult.Success;
        }
    }
}
