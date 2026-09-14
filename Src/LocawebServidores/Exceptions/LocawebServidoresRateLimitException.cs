using System;
using System.Collections.Generic;
using System.Net;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when the API throttles the caller (HTTP 429).
    /// </summary>
    public sealed class LocawebServidoresRateLimitException : LocawebServidoresApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresRateLimitException"/> class.
        /// </summary>
        public LocawebServidoresRateLimitException(
            HttpStatusCode statusCode,
            string message,
            string? responseContent = null,
            IReadOnlyList<JsonApiError>? errors = null,
            IReadOnlyDictionary<string, IEnumerable<string>>? responseHeaders = null,
            TimeSpan? retryAfter = null
        )
            : base(statusCode, message, responseContent, errors, responseHeaders)
        {
            RetryAfter = retryAfter;
        }

        /// <summary>
        /// Gets the delay suggested by the <c>Retry-After</c> response header, when present.
        /// </summary>
        public TimeSpan? RetryAfter { get; }
    }
}
