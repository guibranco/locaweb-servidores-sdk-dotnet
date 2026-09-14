using System.Text.Json;
using System.Text.Json.Serialization;
using LocawebServidores.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API resource object: an <c>id</c>/<c>type</c> pair plus typed
    /// <c>attributes</c>, and the raw <c>meta</c>, <c>relationships</c> and <c>links</c> members.
    /// </summary>
    /// <typeparam name="TAttributes">The type the <c>attributes</c> member is deserialized into.</typeparam>
    public class JsonApiResource<TAttributes>
    {
        /// <summary>
        /// Gets or sets the resource identifier (for example the server key).
        /// </summary>
        [JsonPropertyName("id")]
        [JsonConverter(typeof(LenientStringConverter))]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the resource type (for example <c>servers</c>).
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the typed attributes of the resource.
        /// </summary>
        [JsonPropertyName("attributes")]
        public TAttributes? Attributes { get; set; }

        /// <summary>
        /// Gets or sets the raw <c>relationships</c> member, if any.
        /// </summary>
        [JsonPropertyName("relationships")]
        public JsonElement? Relationships { get; set; }

        /// <summary>
        /// Gets or sets the links related to the resource, if any.
        /// </summary>
        [JsonPropertyName("links")]
        public JsonApiLinks? Links { get; set; }

        /// <summary>
        /// Gets or sets the raw <c>meta</c> member, if any. For servers the API lists
        /// the actions currently available under <c>meta.actions</c>.
        /// </summary>
        [JsonPropertyName("meta")]
        public JsonElement? Meta { get; set; }

        /// <summary>
        /// Tries to read a member of the <see cref="Meta"/> object.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="value">The member value when found.</param>
        /// <returns><c>true</c> when the member exists.</returns>
        public bool TryGetMeta(string name, out JsonElement value)
        {
            if (
                Meta.HasValue
                && Meta.Value.ValueKind == JsonValueKind.Object
                && !string.IsNullOrEmpty(name)
                && Meta.Value.TryGetProperty(name, out value)
            )
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Reads a member of the <see cref="Meta"/> object and deserializes it.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="name">The member name.</param>
        /// <returns>The deserialized value, or <c>default</c> when the member is absent or null.</returns>
        public T? GetMeta<T>(string name)
        {
            if (!TryGetMeta(name, out var element) || element.ValueKind == JsonValueKind.Null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(
                element.GetRawText(),
                LocawebServidoresJson.Default
            );
        }
    }
}
