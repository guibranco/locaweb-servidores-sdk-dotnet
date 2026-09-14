using System;
using System.Collections.Generic;
using System.Net;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when the API answers with a non-success HTTP status code.
    /// More specific subclasses exist for authentication, not found, validation,
    /// rate limiting and server errors.
    /// </summary>
    public class LocawebServidoresApiException : LocawebServidoresException
    {
        private static readonly IReadOnlyDictionary<string, IEnumerable<string>> EmptyHeaders =
            new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the API.</param>
        /// <param name="message">The error message.</param>
        /// <param name="responseContent">The raw response body, if any.</param>
        /// <param name="errors">The JSON:API error objects parsed from the body, if any.</param>
        /// <param name="responseHeaders">The response headers, if any.</param>
        public LocawebServidoresApiException(
            HttpStatusCode statusCode,
            string message,
            string? responseContent = null,
            IReadOnlyList<JsonApiError>? errors = null,
            IReadOnlyDictionary<string, IEnumerable<string>>? responseHeaders = null
        )
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
            Errors = errors ?? Array.Empty<JsonApiError>();
            ResponseHeaders = responseHeaders ?? EmptyHeaders;
        }

        /// <summary>
        /// Gets the HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the raw response body, if any.
        /// </summary>
        public string? ResponseContent { get; }

        /// <summary>
        /// Gets the JSON:API error objects parsed from the response body. Empty when
        /// the body was missing or not a JSON:API error document.
        /// </summary>
        public IReadOnlyList<JsonApiError> Errors { get; }

        /// <summary>
        /// Gets the response headers (case-insensitive keys).
        /// </summary>
        public IReadOnlyDictionary<string, IEnumerable<string>> ResponseHeaders { get; }
    }
}
