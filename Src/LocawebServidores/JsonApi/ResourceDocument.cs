using System.Text.Json.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API document whose primary data is a single resource.
    /// </summary>
    /// <typeparam name="TAttributes">The resource attributes type.</typeparam>
    public sealed class ResourceDocument<TAttributes>
        : JsonApiDocument<JsonApiResource<TAttributes>>
    {
        /// <summary>
        /// Gets the primary resource. Alias of <see cref="JsonApiDocument{TData}.Data"/>.
        /// </summary>
        [JsonIgnore]
        public JsonApiResource<TAttributes>? Resource => Data;
    }
}
