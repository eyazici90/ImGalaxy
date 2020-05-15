using System; 

namespace ImGalaxy.ES.Projector
{
    public class ProjectionNotFoundException : Exception
    {
        public ProjectionNotFoundException(string projectionName)
            : base($"Projection could not be found for {projectionName}")
        {
        }
    }
}
