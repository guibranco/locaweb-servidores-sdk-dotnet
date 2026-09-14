using System.Collections.Generic;
using System.Net;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when the API fails on its side (HTTP 5xx). These requests are usually
    /// safe to retry after a short delay.
    /// </summary>
    public sealed class LocawebServidoresServerErrorException : LocawebServidoresApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresServerErrorException"/> class.
        /// </summary>
        public LocawebServidoresServerErrorException(
            HttpStatusCode statusCode,
            string message,
            string? responseContent = null,
            IReadOnlyList<JsonApiError>? errors = null,
            IReadOnlyDictionary<string, IEnumerable<string>>? responseHeaders = null
        )
            : base(statusCode, message, responseContent, errors, responseHeaders) { }
    }
}
