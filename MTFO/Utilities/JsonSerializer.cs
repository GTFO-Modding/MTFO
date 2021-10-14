using System;
using System.IO;
using System.Text.Json;
using Native = System.Text.Json.JsonSerializer;

namespace MTFO.Utilities
{
    public class JsonSerializer
    {
        public JsonSerializer()
        {
            Options = new()
            {
                AllowTrailingCommas = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            };
        }

        public JsonSerializer(
            bool allowTrailingCommas = true,
            bool includeFields = true,
            bool propertyNameCaseInsensitive = true,
            bool writeIndented = true)
        {
            Options = new()
            {
                AllowTrailingCommas = allowTrailingCommas,
                IncludeFields = includeFields,
                PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = writeIndented
            };
        }
        /// <summary>
        /// Checks if a file exists and will deserialize it using the given type, otherwise it will construct the output and write it to a file
        /// </summary>
        public void TryRead<T>(string path, out T output) where T : new()
        {
            string json;
            if (File.Exists(path))
            {
                json = File.ReadAllText(path);
                output = Deserialize<T>(json);
            }
            else
            {
                output = new();
                json = Serialize(output);
                File.WriteAllText(path, json);
            }
        }

        public string Serialize(object value)
        {
            return Native.Serialize(value, Options);
        }

        public T Deserialize<T>(string json)
        {
            return Native.Deserialize<T>(json, Options);
        }

        public JsonSerializerOptions Options;
    }
}
