﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public interface ISnapshotter
    {
        bool ShouldTakeSnapshot(Type aggregateType, object resolvedEvent);

        Task TakeSnapshotAsync(string stream);
    }
}