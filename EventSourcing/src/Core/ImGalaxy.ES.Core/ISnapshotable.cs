using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotable
    {
        void RestoreSnapshot<TSnapshot>(TSnapshot state);

        TSnapshot TakeSnapshot<TSnapshot>();
    }
}
