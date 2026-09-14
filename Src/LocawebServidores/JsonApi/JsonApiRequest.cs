using System.Text.Json.Serialization;
using LocawebServidores.Internal;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API request document: <c>{ "data": { "type": ..., "id": ..., "attributes": {...} } }</c>.
    /// </summary>
    /// <typeparam name="TAttributes">The attributes type.</typeparam>
    public sealed class JsonApiRequest<TAttributes>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiRequest{TAttributes}"/> class.
        /// </summary>
        /// <param name="type">The JSON:API resource type.</param>
        /// <param name="attributes">The attributes to send.</param>
        /// <param name="id">The resource identifier, required by the specification on updates.</param>
        public JsonApiRequest(string type, TAttributes attributes, string? id = null)
        {
            Data = new JsonApiRequestData<TAttributes>(type, attributes, id);
        }

        /// <summary>
        /// Gets the request's primary data.
        /// </summary>
        [JsonPropertyName("data")]
        public JsonApiRequestData<TAttributes> Data { get; }
    }

    /// <summary>
    /// The <c>data</c> member of a <see cref="JsonApiRequest{TAttributes}"/>.
    /// </summary>
    /// <typeparam name="TAttributes">The attributes type.</typeparam>
    public sealed class JsonApiRequestData<TAttributes>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiRequestData{TAttributes}"/> class.
        /// </summary>
        public JsonApiRequestData(string type, TAttributes attributes, string? id = null)
        {
            Type = Guard.NotNullOrWhiteSpace(type, nameof(type));
            Attributes = attributes;
            Id = id;
        }

        /// <summary>
        /// Gets the JSON:API resource type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; }

        /// <summary>
        /// Gets the resource identifier, if any.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; }

        /// <summary>
        /// Gets the attributes to send.
        /// </summary>
        [JsonPropertyName("attributes")]
        public TAttributes Attributes { get; }
    }
}
