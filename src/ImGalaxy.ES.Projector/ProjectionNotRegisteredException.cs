using System;

namespace ImGalaxy.ES.Projector
{
    public class ProjectionNotRegisteredException : Exception
    {
        public ProjectionNotRegisteredException(string projectionName)
            : base($"Projection did not get registered to projector {projectionName}")
        {
        }
    }
}
