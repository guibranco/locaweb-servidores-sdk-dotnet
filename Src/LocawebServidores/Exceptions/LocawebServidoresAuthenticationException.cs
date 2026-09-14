using System.Collections.Generic;
using System.Net;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when the API rejects the credentials (HTTP 401) or the token lacks
    /// permission for the resource (HTTP 403).
    /// </summary>
    public sealed class LocawebServidoresAuthenticationException : LocawebServidoresApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresAuthenticationException"/> class.
        /// </summary>
        public LocawebServidoresAuthenticationException(
            HttpStatusCode statusCode,
            string message,
            string? responseContent = null,
            IReadOnlyList<JsonApiError>? errors = null,
            IReadOnlyDictionary<string, IEnumerable<string>>? responseHeaders = null
        )
            : base(statusCode, message, responseContent, errors, responseHeaders) { }
    }
}
