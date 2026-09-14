using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using LocawebServidores.Serialization;

namespace LocawebServidores.Http
{
    /// <summary>
    /// A successful raw API response: status code, headers and body.
    /// </summary>
    public sealed class ApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse"/> class.
        /// </summary>
        public ApiResponse(
            HttpStatusCode statusCode,
            string? content,
            IReadOnlyDictionary<string, IEnumerable<string>> headers
        )
        {
            StatusCode = statusCode;
            Content = content;
            Headers =
                headers
                ?? new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the response body, or <c>null</c>/empty when the API sent none.
        /// </summary>
        public string? Content { get; }

        /// <summary>
        /// Gets the response headers (case-insensitive keys), including content headers.
        /// </summary>
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

        /// <summary>
        /// Gets a value indicating whether the response carries a body.
        /// </summary>
        public bool HasContent => !string.IsNullOrWhiteSpace(Content);

        /// <summary>
        /// Deserializes the body using the SDK JSON settings.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The deserialized value, or <c>default</c> when the body is empty.</returns>
        public T? Deserialize<T>()
        {
            return HasContent
                ? JsonSerializer.Deserialize<T>(Content!, LocawebServidoresJson.Default)
                : default;
        }
    }
}
