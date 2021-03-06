﻿using Galaxy.Railway;
using System;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public abstract class CommandHandlerBase<TAggregateRoot, TKey>
        where TAggregateRoot : class, IAggregateRootState<TAggregateRoot>, IAggregateRoot
    {
        public IAggregateRootRepository<TAggregateRoot> RootRepository { get; }

        private readonly Func<Task<IExecutionResult>> _saveChanges;

        private CommandHandlerBase(Func<Task<IExecutionResult>> saveChanges,
            IAggregateRootRepository<TAggregateRoot> rootRepository)
        {
            _saveChanges = saveChanges;
            RootRepository = rootRepository;
        }

        public CommandHandlerBase(IUnitOfWork unitOfWork,
            IAggregateRootRepository<TAggregateRoot> rootRepository)
            : this(async () => await unitOfWork.SaveChangesAsync().ConfigureAwait(false), rootRepository)
        {
        }

        public CommandHandlerBase(IAggregateRootRepository<TAggregateRoot> rootRepository)
             : this(async () => await Task.FromResult(ExecutionResult.Success), rootRepository)
        {
        }

        public virtual async Task<IExecutionResult> AddAsync(Func<Task<TAggregateRoot>> factory, string id) =>
            await AwaitTaskWithPrePostAction(factory, async root => await RootRepository.AddAsync(root, id).ConfigureAwait(false),
                        async root => await _saveChanges().ConfigureAwait(false)).ConfigureAwait(false);

        public virtual async Task<IExecutionResult> UpdateAsync(TKey id, Func<TAggregateRoot, Task> when) =>
            await AwaitTaskWithPrePostAction(async () =>
            {
                var existingRoot = await FindByIdAsync(id).ConfigureAwait(false);
                existingRoot.ThrowsIf(root => !root.HasValue, new AggregateNotFoundException(id.ToString()));
                return existingRoot.Value;
            },
            when,
            async root => await _saveChanges().ConfigureAwait(false)).ConfigureAwait(false);

        private async Task<IExecutionResult> AwaitTaskWithPrePostAction(Func<Task<TAggregateRoot>> preAction,
            Func<TAggregateRoot, Task> realAction,
            Func<TAggregateRoot, Task> postAction)
        {
            var aggregate = await preAction().ConfigureAwait(false);

            await realAction(aggregate).ConfigureAwait(false);

            await postAction(aggregate).ConfigureAwait(false);

            return ExecutionResult.Success;
        }
        private async Task<Optional<TAggregateRoot>> FindByIdAsync(TKey id) => await RootRepository.GetAsync(id.ToString()).ConfigureAwait(false);

    }
}
