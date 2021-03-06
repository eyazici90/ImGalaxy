﻿using System;

namespace ImGalaxy.ES.Core
{
    public interface IEventDeserializer
    {
        T Deserialize<T>(string jsonString, bool camelCase = true);

        object Deserialize(Type type, string jsonString, bool camelCase = true);

        object Deserialize(string jsonString, bool camelCase = true);
    }
}
