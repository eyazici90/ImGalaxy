﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public interface IStreamNameProvider
    {
        string GetStreamName(object aggregateRoot, string identifier);
        string GetStreamName(Type aggregateRootType, string identifier);
        string GetSnapshotStreamName(object aggregateRoot, string identifier);
        string GetSnapshotStreamName(Type aggregateRootType, string identifier);
    }
}
