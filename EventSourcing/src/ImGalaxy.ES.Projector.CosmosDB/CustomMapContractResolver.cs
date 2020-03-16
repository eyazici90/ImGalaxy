using Newtonsoft.Json;
using Newtonsoft.Json.Serialization; 
using System.Reflection; 

namespace ImGalaxy.ES.Projector.CosmosDB
{
    public class CustomMapContractResolver : DefaultContractResolver
    {

        // This is for desializing object for private property setters.
        protected override JsonProperty CreateProperty(
         MemberInfo member,
         MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }
            return prop;
        }
    }
}
