using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTFO.Utilities.JsonConverters
{
    public class PersistentIDConverter : JsonConverter
    {
        public Func<string, uint> SaveID;
        public Func<string, uint> LoadID;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(uint);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JToken token = JToken.Load(reader);
            if (reader.TokenType == JsonToken.String)
            {
                var guidValue = token.ToObject<string>();
                if (reader.Path.EndsWith("persistentID"))
                {
                    if (SaveID == null)
                    {
                        throw new Exception($"SaveID has not implemented!");
                    }

                    return SaveID.Invoke(guidValue);
                }
                else
                {
                    if (LoadID == null)
                    {
                        throw new Exception($"LoadID has not implemented!");
                    }

                    return LoadID.Invoke(guidValue);
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return token.ToObject<uint>();
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
