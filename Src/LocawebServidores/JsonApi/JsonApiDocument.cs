using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API top-level document.
    /// </summary>
    /// <typeparam name="TData">The type of the <c>data</c> member (a resource or a list of resources).</typeparam>
    public class JsonApiDocument<TData>
    {
        /// <summary>
        /// Gets or sets the document's primary data.
        /// </summary>
        [JsonPropertyName("data")]
        public TData? Data { get; set; }

        /// <summary>
        /// Gets or sets the error objects, present only on error documents.
        /// </summary>
        [JsonPropertyName("errors")]
        public IReadOnlyList<JsonApiError>? Errors { get; set; }

        /// <summary>
        /// Gets or sets the raw <c>meta</c> member, if any.
        /// </summary>
        [JsonPropertyName("meta")]
        public JsonElement? Meta { get; set; }

        /// <summary>
        /// Gets or sets the top-level links (including pagination links), if any.
        /// </summary>
        [JsonPropertyName("links")]
        public JsonApiLinks? Links { get; set; }

        /// <summary>
        /// Gets or sets the included resources of a compound document, if any.
        /// </summary>
        [JsonPropertyName("included")]
        public IReadOnlyList<JsonApiResource<JsonElement>>? Included { get; set; }

        /// <summary>
        /// Gets or sets the raw <c>jsonapi</c> member, if any.
        /// </summary>
        [JsonPropertyName("jsonapi")]
        public JsonElement? JsonApi { get; set; }
    }
}
