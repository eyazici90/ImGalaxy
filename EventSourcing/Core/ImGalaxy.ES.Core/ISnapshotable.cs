using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotable<TSnapshot> where TSnapshot : class
    {
        void RestoreSnapshot(TSnapshot state);

        TSnapshot TakeSnapshot();
    }
}
