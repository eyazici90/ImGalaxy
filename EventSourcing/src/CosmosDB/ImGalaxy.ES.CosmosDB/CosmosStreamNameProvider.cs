﻿using ImGalaxy.ES.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.CosmosDB
{
    public class CosmosStreamNameProvider : IStreamNameProvider
    {
        public string GetSnapshotStreamName(object aggregateRoot, string identifier) =>
          $"{GetStreamName(aggregateRoot, identifier)}-Snapshot";
        public string GetSnapshotStreamName(Type aggregateRootType, string identifier) =>
             $"{GetStreamName(aggregateRootType, identifier)}-Snapshot";

        public string GetStreamName(object aggregateRoot, string identifier) =>
          CosmosStreamNameExtensions.GetFullStreamName(aggregateRoot.GetType().ToString(), identifier);
        public string GetStreamName(Type aggregateRootType, string identifier)=>
            CosmosStreamNameExtensions.GetFullStreamName(aggregateRootType.ToString(), identifier);
    }
}
