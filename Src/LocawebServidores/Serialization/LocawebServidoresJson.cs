using System.Text.Json;
using System.Text.Json.Serialization;

namespace LocawebServidores.Serialization
{
    /// <summary>
    /// Provides the JSON serializer settings used by the SDK when talking to the
    /// Locaweb Servidores API (snake_case property names, lenient number parsing,
    /// nulls omitted on write).
    /// </summary>
    public static class LocawebServidoresJson
    {
        /// <summary>
        /// The shared, read-only settings instance used internally by the SDK.
        /// </summary>
        internal static JsonSerializerOptions Default { get; } = CreateOptions();

        /// <summary>
        /// Creates a new <see cref="JsonSerializerOptions"/> instance configured exactly
        /// like the one the SDK uses, so callers can deserialize raw
        /// <see cref="JsonElement"/> payloads (for example additional attributes) into
        /// their own types consistently.
        /// </summary>
        /// <returns>A new, mutable options instance.</returns>
        public static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
        }
    }
}
