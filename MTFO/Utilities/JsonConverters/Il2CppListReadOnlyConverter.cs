using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTFO.Utilities.JsonConverters
{
    using Il2CppCollections = Il2CppSystem.Collections.Generic;

    public class Il2CppListReadOnlyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType)
            {
                bool flag1 = objectType.GetGenericTypeDefinition() == typeof(Il2CppCollections.List<>);
                bool flag2 = objectType.GetGenericArguments().Length == 1;
                return flag1 && flag2;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var argTypes = objectType.GetGenericArguments();
            var argType = objectType.GetGenericArguments()[0];

            var listGenericType = typeof(Il2CppCollections.List<>);
            var listType = listGenericType.MakeGenericType(argTypes);

            dynamic list = Activator.CreateInstance(listType);

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                JToken token = JToken.Load(reader);
                foreach (var a in (token as JArray).ToArray())
                {
                    dynamic obj = a.ToObject(argType, serializer);
                    list.Add(obj);
                }

                return list;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            return;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
