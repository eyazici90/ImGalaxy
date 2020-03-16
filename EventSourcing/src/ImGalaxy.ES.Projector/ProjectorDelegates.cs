using System; 
using System.Threading.Tasks;

namespace ImGalaxy.ES.Projector
{
    public class ProjectorDelegates
    {
        public delegate Task UpdateOrInsert(object @obj);

        public delegate Task<dynamic> Get(string id);

        public delegate object FindProjection(Type type);
    }
}
