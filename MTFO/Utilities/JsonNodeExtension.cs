using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTFO
{
    public static class JsonNodeExtension
    {
        private static JsonDocumentOptions _JsonDocumentOptions = new()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };

        private static readonly JsonSerializerOptions _JsonSerializerOptions = new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string ToJsonStringIndented(this JsonNode jsonNode)
        {
            return jsonNode.ToJsonString(_JsonSerializerOptions);
        }

        public static bool TryParseToJsonNode(this string json, out JsonNode jsonNode)
        {
            try
            {
                jsonNode = json.ToJsonNode();
                return jsonNode != null;
            }
            catch
            {
                jsonNode = null;
                return false;
            }
        }

        public static bool TryParseJsonNode(this Stream stream, bool dispose, out JsonNode jsonNode)
        {
            try
            {
                jsonNode = stream.ToJsonNode();
                return jsonNode != null;
            }
            catch
            {
                jsonNode = null;
                return false;
            }
            finally
            {
                if (dispose)
                {
                    stream.Close();
                }
            }
        }

        public static JsonNode ToJsonNode(this string json)
        {
            return JsonNode.Parse(json, null, _JsonDocumentOptions);
        }

        public static JsonNode ToJsonNode(this Stream jsonStream)
        {
            return JsonNode.Parse(jsonStream, null, _JsonDocumentOptions);
        }

        public static bool TryAddJsonItem(this JsonArray jsonArray, string json)
        {
            try
            {
                if (jsonArray == null)
                {
                    throw new Exception();
                }

                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new Exception();
                }

                if (!json.TryParseToJsonNode(out var node))
                {
                    throw new Exception();
                }

                jsonArray.Add(node);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
