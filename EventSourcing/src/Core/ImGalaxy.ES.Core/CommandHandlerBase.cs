using System;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public abstract class CommandHandlerBase<TAggregateRoot, TKey> 
        where TAggregateRoot :  IAggregateRoot
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IAggregateRootRepository<TAggregateRoot> _rootRepository;
        public CommandHandlerBase(IUnitOfWork unitOfWork, IAggregateRootRepository<TAggregateRoot> rootRepository)
        {
            _unitOfWork = unitOfWork;
            _rootRepository = rootRepository;
        }
        public virtual async Task AddAsync(Func<Task<TAggregateRoot>> factory, string id) =>
            await AwaitTaskWithPrePostAction(factory, async root => await _rootRepository.AddAsync(root, id),
                        async root => await _unitOfWork.SaveChangesAsync());

     
        public virtual async Task UpdateAsync(TKey id, Func<TAggregateRoot, Task> when) =>
            await AwaitTaskWithPrePostAction(async () =>
            {
                var existingRoot = await FindByIdAsync(id);
                existingRoot.ThrowsIf(root => !root.HasValue, new AggregateNotFoundException(id.ToString()));
                return existingRoot.Value;
            },
            async a => await when(a),
            async root => await _unitOfWork.SaveChangesAsync());


        private async Task AwaitTaskWithPrePostAction(Func<Task<TAggregateRoot>> preAction,
            Func<TAggregateRoot, Task> realAction, 
            Func<TAggregateRoot, Task> postAction)
        {
            var aggregate = await preAction();

            await realAction(aggregate);

            await postAction(aggregate);
        }
        private async Task<Optional<TAggregateRoot>> FindByIdAsync(TKey id) => await _rootRepository.GetAsync(id.ToString()); 

    }
}
