using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LocawebServidores.Http
{
    /// <summary>
    /// Low-level access to the Locaweb Servidores API: authentication, serialization and
    /// error mapping. Use it directly to call endpoints this SDK does not model.
    /// </summary>
    public interface IApiConnection
    {
        /// <summary>
        /// Gets the client options in use.
        /// </summary>
        LocawebServidoresClientOptions Options { get; }

        /// <summary>
        /// Sends a GET request and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="path">The path relative to the base address, for example <c>cloud/servers</c>.</param>
        /// <param name="options">Optional query parameters and locale.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<T> GetAsync<T>(
            string path,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sends a POST request with a JSON body and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="path">The path relative to the base address.</param>
        /// <param name="body">The object serialized as the request body, or <c>null</c> for no body.</param>
        /// <param name="options">Optional query parameters and locale.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<T> PostAsync<T>(
            string path,
            object? body = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sends a PUT request with a JSON body and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="path">The path relative to the base address.</param>
        /// <param name="body">The object serialized as the request body, or <c>null</c> for no body.</param>
        /// <param name="options">Optional query parameters and locale.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<T> PutAsync<T>(
            string path,
            object? body = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sends a DELETE request, ignoring any response body.
        /// </summary>
        /// <param name="path">The path relative to the base address.</param>
        /// <param name="options">Optional query parameters and locale.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task DeleteAsync(
            string path,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sends a request and returns the raw successful response. Non-success status
        /// codes are converted into <see cref="Exceptions.LocawebServidoresApiException"/>
        /// (or a subclass).
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The path relative to the base address.</param>
        /// <param name="body">The object serialized as the request body, or <c>null</c> for no body.</param>
        /// <param name="options">Optional query parameters and locale.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task<ApiResponse> SendAsync(
            HttpMethod method,
            string path,
            object? body = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
