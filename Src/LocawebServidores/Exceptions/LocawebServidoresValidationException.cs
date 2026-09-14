using System.Collections.Generic;
using System.Net;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when the API rejects the request payload or parameters
    /// (HTTP 400 or 422). Inspect <see cref="LocawebServidoresApiException.Errors"/>
    /// for the individual JSON:API error objects.
    /// </summary>
    public sealed class LocawebServidoresValidationException : LocawebServidoresApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresValidationException"/> class.
        /// </summary>
        public LocawebServidoresValidationException(
            HttpStatusCode statusCode,
            string message,
            string? responseContent = null,
            IReadOnlyList<JsonApiError>? errors = null,
            IReadOnlyDictionary<string, IEnumerable<string>>? responseHeaders = null
        )
            : base(statusCode, message, responseContent, errors, responseHeaders) { }
    }
}
