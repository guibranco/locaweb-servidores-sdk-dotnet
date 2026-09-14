using System.Text.Json.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// The JSON:API <c>source</c> member of an error object, pointing at the part of
    /// the request that caused the problem.
    /// </summary>
    public sealed class JsonApiErrorSource
    {
        /// <summary>
        /// Gets or sets a JSON pointer to the offending member of the request document
        /// (for example <c>/data/attributes/nickname</c>).
        /// </summary>
        [JsonPropertyName("pointer")]
        public string? Pointer { get; set; }

        /// <summary>
        /// Gets or sets the name of the offending query parameter.
        /// </summary>
        [JsonPropertyName("parameter")]
        public string? Parameter { get; set; }

        /// <summary>
        /// Gets or sets the name of the offending request header.
        /// </summary>
        [JsonPropertyName("header")]
        public string? Header { get; set; }
    }
}
