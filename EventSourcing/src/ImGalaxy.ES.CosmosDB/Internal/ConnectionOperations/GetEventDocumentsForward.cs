using ImGalaxy.ES.CosmosDB.Documents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ImGalaxy.ES.CosmosDB.Internal.ConnectionOperations
{
    internal class GetEventDocumentsForward
    { 
        internal Expression<Func<EventDocument, bool>> Predicate { get; }
        internal int Start { get; }
        internal int Count { get; } 
        internal GetEventDocumentsForward(Expression<Func<EventDocument, bool>> predicate,
            int start,
            int count)
        {
            Predicate = predicate;
            Start = start;
            Count = count;
        }
    }
}
