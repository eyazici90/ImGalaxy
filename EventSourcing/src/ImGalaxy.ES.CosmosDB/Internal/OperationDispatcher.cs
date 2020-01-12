using System;
using System.Collections.Concurrent; 
using System.Threading.Tasks;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal class OperationDispatcher : IOperationDispatcher
    {
        private readonly ConcurrentDictionary<Type, object> _handlers = new ConcurrentDictionary<Type, object>();
        private readonly Func<Type, object> _serviceFactory;
        public OperationDispatcher(Func<Type, object> serviceFactory) =>
            _serviceFactory = serviceFactory;

        async Task<IExecutionResult> IOperationDispatcher.Dispatch<TOperation>(TOperation operation)
        {
            var handler = _handlers.GetOrAdd(typeof(TOperation), _serviceFactory(typeof(TOperation)));

            handler.ThrowsIfNull(new ArgumentNullException($"handler could not found type of {typeof(TOperation).Name}"));

            await (handler as IOperationHandler<TOperation>).Handle(operation);

            return ExecutionResult.Success;
        } 
    }
}
