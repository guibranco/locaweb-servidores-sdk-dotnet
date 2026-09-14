using System.Collections.Generic;
using System.Net;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when the requested resource does not exist (HTTP 404).
    /// </summary>
    public sealed class LocawebServidoresNotFoundException : LocawebServidoresApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresNotFoundException"/> class.
        /// </summary>
        public LocawebServidoresNotFoundException(
            HttpStatusCode statusCode,
            string message,
            string? responseContent = null,
            IReadOnlyList<JsonApiError>? errors = null,
            IReadOnlyDictionary<string, IEnumerable<string>>? responseHeaders = null
        )
            : base(statusCode, message, responseContent, errors, responseHeaders) { }
    }
}
