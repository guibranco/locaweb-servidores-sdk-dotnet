using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API document whose primary data is a list of resources, as returned by
    /// the "list" endpoints. Pagination links, when provided by the API, are exposed
    /// through <see cref="JsonApiDocument{TData}.Links"/>.
    /// </summary>
    /// <typeparam name="TAttributes">The resource attributes type.</typeparam>
    public sealed class ResourceCollection<TAttributes>
        : JsonApiDocument<IReadOnlyList<JsonApiResource<TAttributes>>>
    {
        /// <summary>
        /// Gets the resources in the document. Never <c>null</c>.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<JsonApiResource<TAttributes>> Items =>
            Data ?? Array.Empty<JsonApiResource<TAttributes>>();

        /// <summary>
        /// Gets the number of resources in the document.
        /// </summary>
        [JsonIgnore]
        public int Count => Items.Count;

        /// <summary>
        /// Gets a value indicating whether the document has a <c>next</c> pagination link.
        /// </summary>
        [JsonIgnore]
        public bool HasNextPage => !string.IsNullOrEmpty(Links?.Next);
    }
}
