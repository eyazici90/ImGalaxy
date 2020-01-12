using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal
{
    internal interface IOperationDispatcher
    {
        Task<IExecutionResult> Dispatch<TOperation>(TOperation operation); 
    }
}
