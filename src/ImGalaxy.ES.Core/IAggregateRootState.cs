namespace ImGalaxy.ES.Core
{
    public interface IAggregateRootState<TState> : IEntityState<TState>
    {
        string GetStreamName(string id); 
    }
}
