﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ImGalaxy.ES.Core;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal class OperationDispatcher : IOperationDispatcher
    {
        private readonly ConcurrentDictionary<Type, Func<object>> _handlers = new ConcurrentDictionary<Type, Func<object>>();
        private readonly ConcurrentDictionary<Type, Func<object>> _handlerPipelines = new ConcurrentDictionary<Type, Func<object>>();

        IExecutionResult IOperationDispatcher.RegisterHandler<TOperation>(Func<object> handler)
        {
            _handlers.TryAdd(typeof(TOperation), handler);

            return ExecutionResult.Success;
        }
        IExecutionResult IOperationDispatcher.RegisterPipeline<TOperation>(Func<object> handler)
        {
            _handlerPipelines.TryAdd(typeof(TOperation), handler);

            return ExecutionResult.Success;
        }

        async Task<IExecutionResult> IOperationDispatcher.Dispatch<TOperation>(TOperation operation) =>
            await (this as IOperationDispatcher).Dispatch<TOperation, IExecutionResult>(operation);

        async Task<IExecutionResult> IOperationDispatcher.Dispatch(object operation) =>
            await (this as IOperationDispatcher).Dispatch(operation);

        async Task<TResult> IOperationDispatcher.Dispatch<TOperation, TResult>(TOperation operation)
        {
            _handlers.TryGetValue(typeof(TOperation), out var handler);

            handler.ThrowsIfNull(new ArgumentNullException($"handler type could not found of {typeof(TOperation).Name}"));

            _handlerPipelines.TryGetValue(typeof(TOperation), out var handlerPipeline);

            Func<TOperation, Task<TResult>> handleOpt = async oprt => await (handler() as IOperationHandler<TOperation, TResult>).Handle(oprt);

            if (handlerPipeline is null)
                return await handleOpt(operation);

            else
                return await (handlerPipeline() as IOperationPipeline<TOperation, TResult>)
                     .Handle(operation, async opt => await handleOpt(opt));
        }
    }
}
