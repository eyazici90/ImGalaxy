using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImGalaxy.ES.CosmosDB.Internal.Extensions
{
    public static class CosmosClientExtensions
    {
        public static async Task ToFeedIterator<T>(this IQueryable<T> queryable, Func<T, Task> func)
        {
            var iterator = queryable.ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    await func(item).ConfigureAwait(false);
                }
            }
        }

        public static async Task ToFeedIterator<T>(this IQueryable<T> queryable, Action<T> action)
        {
            var iterator = queryable.ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    action(item);
                }
            }
        }
    }
}
