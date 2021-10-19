using System;
using System.IO;
using System.Text.Json;
using Native = System.Text.Json.JsonSerializer;

namespace MTFO.Utilities
{
    /// <summary>
    /// A serializer for deserializing JSON.
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Creates a new serializater, with the default options:
        /// <list type="bullet">
        /// <item>
        /// <term>Allow trailing commas</term>
        /// <description><see langword="true"/></description>
        /// </item>
        /// <item>
        /// <term>Include Fields</term>
        /// <description><see langword="true"/></description>
        /// </item>
        /// <item>
        /// <term>Property Names are Case Sensitive</term>
        /// <description><see langword="false"/></description>
        /// </item>
        /// <item>
        /// <term>Comment Handling</term>
        /// <description>Skip</description>
        /// </item>
        /// <item>
        /// <term>Write Indented</term>
        /// <description><see langword="true"/></description>
        /// </item>
        /// </list>
        /// </summary>
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

        /// <summary>
        /// Creates a new JSON serializer with the given properties
        /// </summary>
        /// <param name="allowTrailingCommas">Whether or not to allow trailing commas in the json</param>
        /// <param name="includeFields">Whether or not to include fields to write to when deserializing/serializing
        /// the json</param>
        /// <param name="propertyNameCaseInsensitive">Whether or not the properties aren't case sensitive. If set to
        /// false, the property names in the json must match the property names on the types.</param>
        /// <param name="writeIndented">Whether or not to write in an indented format when serializing json.</param>
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
        /// <param name="path">The path of the file</param>
        /// <param name="output">The output of the serialization</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
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

        /// <summary>
        /// Serializes the given JSON string to an object
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns></returns>
        public string Serialize(object value)
        {
            return Native.Serialize(value, Options);
        }

        /// <summary>
        /// Deserializes the given JSON, returning <typeparamref name="T"/>, containing the data
        /// in <paramref name="json"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON to deserialize</param>
        /// <returns>The deserialized JSON</returns>
        /// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/></exception>
        /// <exception cref="JsonException">Thrown if given invalid json</exception>
        public T Deserialize<T>(string json)
        {
            try
            {
                return Native.Deserialize<T>(json, Options);
            }
            catch (JsonException serializeException)
            {
                Log.Warn($"Failed to deserialize JSON at line {serializeException.LineNumber}, character {serializeException.BytePositionInLine}: {serializeException.Message}");

                // throw for handlers to catch
                throw serializeException;
            }
        }

        /// <summary>
        /// Deserializes the JSON content found in the given file.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="path">The path of the file</param>
        /// <returns>The deserialized content</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">Thrown if the file at <paramref name="path"/> doesn't exist</exception>
        /// <exception cref="JsonException">Thrown if parsing the JSON failed.</exception>
        public T DeserializeFromFile<T>(string path)
        {
            try
            {
                return Native.Deserialize<T>(File.ReadAllText(path), Options);
            }
            catch (JsonException serializeException)
            {
                Log.Warn($"Failed to deserialize JSON at path '{path}'; line {serializeException.LineNumber}, character {serializeException.BytePositionInLine}: {serializeException.Message}");

                // throw for handlers to catch
                throw serializeException;
            }
        }

        /// <summary>
        /// The options for deserialization
        /// </summary>
        public JsonSerializerOptions Options;
    }
}
