using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.EventStore
{ 
    public interface ICheckpointStore
    {
        Task<T> GetLastCheckpoint<T>(string projection);

        Task SetLastCheckpoint<T>(string projection, T checkpoint);
    }
}
