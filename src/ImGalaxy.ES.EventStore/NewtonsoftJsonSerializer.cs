﻿using ImGalaxy.ES.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.EventStore
{
    public class NewtonsoftJsonSerializer : IEventSerializer, IEventDeserializer
    {
        public string Serialize(object obj, bool camelCase = true, bool indented = false) =>
            JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));


        public T Deserialize<T>(string jsonString, bool camelCase = true) =>
            JsonConvert.DeserializeObject<T>(jsonString, CreateSerializerSettings(camelCase));


        public object Deserialize(string jsonString, bool camelCase = true) =>
            JsonConvert.DeserializeObject(jsonString, CreateSerializerSettings(camelCase));


        public object Deserialize(Type type, string jsonString, bool camelCase = true) =>
           JsonConvert.DeserializeObject(jsonString, type, CreateSerializerSettings(camelCase));


        private JsonSerializerSettings CreateSerializerSettings(bool camelCase = true, bool indented = false)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.None;
            settings.NullValueHandling = NullValueHandling.Ignore;

            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }

            if (camelCase)
            {
                settings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            }
            return settings;
        }
    }
}
