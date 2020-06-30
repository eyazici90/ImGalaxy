using Galaxy.Railway; 

namespace ImGalaxy.ES.Core
{
    public interface IPolicy<TResult, T>
    {
        TResult Apply(T policy);
    } 
    public interface IPolicy<T>
    {
        IExecutionResult Apply(T policy);
    } 

}
