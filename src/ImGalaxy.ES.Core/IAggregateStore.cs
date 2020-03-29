using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateStore
    {
        Task<IExecutionResult> Save<T>(
           string identifer,
           Version version,
           AggregateRootState<T>.Result update)
           where T : class, IAggregateRootState<T>;

        Task<IExecutionResult> Save(Aggregate aggregate);
        Task<Aggregate> Load<T>(string id, int version = default)
            where T : class, IAggregateRootState<T>;
    }
}
