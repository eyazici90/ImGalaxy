using System; 
namespace ImGalaxy.ES.Projector
{
    public class ProjectionHandlerNotFoundException : Exception
    {
        public ProjectionHandlerNotFoundException(string handlerType)
            : base($"handler type could not found of {handlerType}")
        {
        }
    }
}
