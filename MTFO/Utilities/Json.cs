using System;
using System.IO;
using System.Text.Json;

namespace MTFO.Utilities
{
    public static class Json
    {
        /// <summary>
        /// Checks if a file exists and will deserialize it using the given type, otherwise it will construct the output and write it to a file
        /// </summary>
        public static void TryRead<T>(string path, out T output) where T : new()
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

        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, options);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        private static JsonSerializerOptions options = new() {
            AllowTrailingCommas = true,
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true
        };
    }
}
