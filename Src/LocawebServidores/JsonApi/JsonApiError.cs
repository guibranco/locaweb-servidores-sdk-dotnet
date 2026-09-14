using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LocawebServidores.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API error object.
    /// </summary>
    public sealed class JsonApiError
    {
        /// <summary>
        /// Gets or sets a unique identifier for this occurrence of the problem.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonConverter(typeof(LenientStringConverter))]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code applicable to this problem.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(LenientStringConverter))]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets an application-specific error code.
        /// </summary>
        [JsonPropertyName("code")]
        [JsonConverter(typeof(LenientStringConverter))]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets a short, human-readable summary of the problem.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets a human-readable explanation specific to this occurrence.
        /// </summary>
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        /// <summary>
        /// Gets or sets the reference to the source of the error.
        /// </summary>
        [JsonPropertyName("source")]
        public JsonApiErrorSource? Source { get; set; }

        /// <summary>
        /// Gets or sets links related to the error.
        /// </summary>
        [JsonPropertyName("links")]
        public JsonApiLinks? Links { get; set; }

        /// <summary>
        /// Gets or sets non-standard meta-information about the error.
        /// </summary>
        [JsonPropertyName("meta")]
        public JsonElement? Meta { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Status))
            {
                builder.Append('[').Append(Status).Append("] ");
            }

            if (!string.IsNullOrWhiteSpace(Title))
            {
                builder.Append(Title);
            }

            if (!string.IsNullOrWhiteSpace(Detail))
            {
                if (builder.Length > 0 && !string.IsNullOrWhiteSpace(Title))
                {
                    builder.Append(": ");
                }

                builder.Append(Detail);
            }

            var pointer = Source?.Pointer ?? Source?.Parameter;
            if (!string.IsNullOrWhiteSpace(pointer))
            {
                builder.Append(" (").Append(pointer).Append(')');
            }

            return builder.Length == 0 ? Code ?? "Unknown error" : builder.ToString().Trim();
        }
    }
}
