using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootRepository<TAggregateRoot> 
        where TAggregateRoot : class, IAggregateRootState<TAggregateRoot>, IAggregateRoot
    {
        Optional<TAggregateRoot>  Get(string identifier);
        Task<Optional<TAggregateRoot>> GetAsync(string identifier); 
        void Add(TAggregateRoot root, string identifier); 
        Task AddAsync(TAggregateRoot root, string identifier); 
    }
}
