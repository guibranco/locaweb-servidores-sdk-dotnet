using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.Internal;
using LocawebServidores.Serialization;

namespace LocawebServidores.Http
{
    /// <summary>
    /// Default <see cref="IApiConnection"/> implementation built on <see cref="HttpClient"/>.
    /// Headers are set per request, so a shared or factory-managed <see cref="HttpClient"/>
    /// is never mutated.
    /// </summary>
    public sealed class ApiConnection : IApiConnection
    {
        private const string JsonMediaType = "application/json";
        private const string LocaleParameterName = "locale";

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConnection"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        /// <param name="options">The client options; validated on construction.</param>
        public ApiConnection(HttpClient httpClient, LocawebServidoresClientOptions options)
        {
            _httpClient = Guard.NotNull(httpClient, nameof(httpClient));
            Options = Guard.NotNull(options, nameof(options));
            Options.Validate();
            _serializerOptions = LocawebServidoresJson.Default;
        }

        /// <inheritdoc />
        public LocawebServidoresClientOptions Options { get; }

        /// <inheritdoc />
        public Task<T> GetAsync<T>(
            string path,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return SendAndDeserializeAsync<T>(
                HttpMethod.Get,
                path,
                null,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<T> PostAsync<T>(
            string path,
            object? body = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return SendAndDeserializeAsync<T>(
                HttpMethod.Post,
                path,
                body,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<T> PutAsync<T>(
            string path,
            object? body = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return SendAndDeserializeAsync<T>(
                HttpMethod.Put,
                path,
                body,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task DeleteAsync(
            string path,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            await SendAsync(HttpMethod.Delete, path, null, options, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ApiResponse> SendAsync(
            HttpMethod method,
            string path,
            object? body = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(method, nameof(method));
            Guard.NotNullOrWhiteSpace(path, nameof(path));

            using (var request = CreateRequest(method, path, body, options))
            {
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient
                        .SendAsync(
                            request,
                            HttpCompletionOption.ResponseContentRead,
                            cancellationToken
                        )
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException exception)
                {
                    throw new LocawebServidoresException(
                        "The request to '"
                            + request.RequestUri
                            + "' could not be completed: "
                            + exception.Message,
                        exception
                    );
                }
                catch (TaskCanceledException exception)
                    when (!cancellationToken.IsCancellationRequested)
                {
                    throw new LocawebServidoresTimeoutException(
                        "The request to '"
                            + request.RequestUri
                            + "' timed out after "
                            + _httpClient.Timeout
                            + ".",
                        exception
                    );
                }

                using (response)
                {
                    var content = response.Content is null
                        ? null
                        : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var headers = CopyHeaders(response);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw ApiExceptionFactory.Create(
                            response.StatusCode,
                            response.ReasonPhrase,
                            content,
                            headers,
                            _serializerOptions
                        );
                    }

                    return new ApiResponse(response.StatusCode, content, headers);
                }
            }
        }

        /// <summary>
        /// Builds the absolute request URI for a relative path, appending the query
        /// parameters and locale from <paramref name="options"/> and the client options.
        /// </summary>
        /// <param name="path">The path relative to the base address.</param>
        /// <param name="options">Optional per-request options.</param>
        /// <returns>The absolute URI.</returns>
        public Uri BuildUri(string path, RequestOptions? options = null)
        {
            Guard.NotNullOrWhiteSpace(path, nameof(path));

            var baseAddress = Options.BaseAddress.AbsoluteUri;
            if (!baseAddress.EndsWith("/", StringComparison.Ordinal))
            {
                baseAddress += "/";
            }

            var relative = QueryStringBuilder.Append(
                path.TrimStart('/'),
                BuildQueryParameters(options)
            );
            return new Uri(new Uri(baseAddress), relative);
        }

        private IEnumerable<KeyValuePair<string, string>>? BuildQueryParameters(
            RequestOptions? options
        )
        {
            var locale = options?.Locale ?? Options.Locale;
            var parameters = options?.QueryParameters;
            if (string.IsNullOrWhiteSpace(locale))
            {
                return parameters;
            }

            var result = new List<KeyValuePair<string, string>>();
            if (parameters != null)
            {
                result.AddRange(parameters);
            }

            result.Add(new KeyValuePair<string, string>(LocaleParameterName, locale!));
            return result;
        }

        private async Task<T> SendAndDeserializeAsync<T>(
            HttpMethod method,
            string path,
            object? body,
            RequestOptions? options,
            CancellationToken cancellationToken
        )
        {
            var response = await SendAsync(method, path, body, options, cancellationToken)
                .ConfigureAwait(false);

            if (!response.HasContent)
            {
                throw new LocawebServidoresException(
                    "The API returned an empty body (HTTP "
                        + (int)response.StatusCode
                        + ") where a JSON document was expected."
                );
            }

            T? result;
            try
            {
                result = JsonSerializer.Deserialize<T>(response.Content!, _serializerOptions);
            }
            catch (JsonException exception)
            {
                throw new LocawebServidoresException(
                    "The API response could not be parsed as JSON: " + exception.Message,
                    exception
                );
            }

            if (result is null)
            {
                throw new LocawebServidoresException(
                    "The API returned a JSON null where a document was expected."
                );
            }

            return result;
        }

        private HttpRequestMessage CreateRequest(
            HttpMethod method,
            string path,
            object? body,
            RequestOptions? options
        )
        {
            var request = new HttpRequestMessage(method, BuildUri(path, options));
            try
            {
                request.Headers.TryAddWithoutValidation(
                    Options.AuthenticationHeaderName,
                    BuildAuthenticationValue()
                );
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Options.MediaType));
                if (
                    !string.Equals(
                        Options.MediaType,
                        JsonMediaType,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
                }

                if (!string.IsNullOrWhiteSpace(Options.UserAgent))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", Options.UserAgent);
                }

                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body, body.GetType(), _serializerOptions);
                    var content = new StringContent(json, Encoding.UTF8);
                    // JSON:API requires the media type without parameters (no charset).
                    content.Headers.ContentType = new MediaTypeHeaderValue(Options.MediaType);
                    request.Content = content;
                }

                return request;
            }
            catch
            {
                request.Dispose();
                throw;
            }
        }

        private string BuildAuthenticationValue()
        {
            return string.IsNullOrWhiteSpace(Options.AuthenticationScheme)
                ? Options.ApiToken
                : Options.AuthenticationScheme!.Trim() + " " + Options.ApiToken;
        }

        private static IReadOnlyDictionary<string, IEnumerable<string>> CopyHeaders(
            HttpResponseMessage response
        )
        {
            var headers = new Dictionary<string, IEnumerable<string>>(
                StringComparer.OrdinalIgnoreCase
            );

            foreach (var header in response.Headers)
            {
                headers[header.Key] = new List<string>(header.Value);
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    headers[header.Key] = new List<string>(header.Value);
                }
            }

            return headers;
        }
    }
}
