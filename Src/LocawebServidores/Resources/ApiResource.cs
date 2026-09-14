using System;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.Http;
using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Base class for the typed resource clients. Wraps <see cref="IApiConnection"/>
    /// with JSON:API document handling and path helpers.
    /// </summary>
    public abstract class ApiResource
    {
        /// <summary>
        /// The default interval between polls in the <c>WaitForCompletionAsync</c> helpers.
        /// </summary>
        public static readonly TimeSpan DefaultPollInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        /// <param name="connection">The API connection.</param>
        protected ApiResource(IApiConnection connection)
        {
            Connection = Guard.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Gets the underlying API connection.
        /// </summary>
        protected IApiConnection Connection { get; }

        /// <summary>
        /// Validates and URL-escapes a path segment.
        /// </summary>
        protected static string Segment(string value, string paramName)
        {
            return Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(value, paramName));
        }

        /// <summary>
        /// Builds the <c>cloud/servers/{serverKey}</c> path.
        /// </summary>
        protected static string CloudServerPath(string serverKey)
        {
            return "cloud/servers/" + Segment(serverKey, nameof(serverKey));
        }

        /// <summary>
        /// Performs a GET returning a list of resources.
        /// </summary>
        protected Task<ResourceCollection<TAttributes>> GetCollectionAsync<TAttributes>(
            string path,
            RequestOptions? options,
            CancellationToken cancellationToken
        )
        {
            return Connection.GetAsync<ResourceCollection<TAttributes>>(
                path,
                options,
                cancellationToken
            );
        }

        /// <summary>
        /// Performs a GET returning a single resource.
        /// </summary>
        protected async Task<JsonApiResource<TAttributes>> GetResourceAsync<TAttributes>(
            string path,
            RequestOptions? options,
            CancellationToken cancellationToken
        )
        {
            var document = await Connection
                .GetAsync<ResourceDocument<TAttributes>>(path, options, cancellationToken)
                .ConfigureAwait(false);
            return RequireData(document);
        }

        /// <summary>
        /// Performs a POST with a JSON:API request document, returning the created resource.
        /// </summary>
        protected async Task<JsonApiResource<TAttributes>> PostResourceAsync<TAttributes, TRequest>(
            string path,
            string resourceType,
            TRequest attributes,
            RequestOptions? options,
            CancellationToken cancellationToken
        )
        {
            var body = new JsonApiRequest<TRequest>(resourceType, attributes);
            var document = await Connection
                .PostAsync<ResourceDocument<TAttributes>>(path, body, options, cancellationToken)
                .ConfigureAwait(false);
            return RequireData(document);
        }

        /// <summary>
        /// Performs a PUT with a JSON:API request document, returning the updated resource.
        /// </summary>
        protected async Task<JsonApiResource<TAttributes>> PutResourceAsync<TAttributes, TRequest>(
            string path,
            string resourceType,
            string? id,
            TRequest attributes,
            RequestOptions? options,
            CancellationToken cancellationToken
        )
        {
            var body = new JsonApiRequest<TRequest>(resourceType, attributes, id);
            var document = await Connection
                .PutAsync<ResourceDocument<TAttributes>>(path, body, options, cancellationToken)
                .ConfigureAwait(false);
            return RequireData(document);
        }

        /// <summary>
        /// Repeatedly fetches a resource until <paramref name="isCompleted"/> returns
        /// <c>true</c>, the optional timeout elapses, or the token is cancelled.
        /// </summary>
        /// <exception cref="LocawebServidoresTimeoutException">The timeout elapsed.</exception>
        /// <exception cref="OperationCanceledException">The caller's token was cancelled.</exception>
        protected static async Task<JsonApiResource<TAttributes>> PollUntilAsync<TAttributes>(
            Func<CancellationToken, Task<JsonApiResource<TAttributes>>> fetch,
            Func<JsonApiResource<TAttributes>, bool> isCompleted,
            TimeSpan? pollInterval,
            TimeSpan? timeout,
            CancellationToken cancellationToken
        )
        {
            Guard.NotNull(fetch, nameof(fetch));
            Guard.NotNull(isCompleted, nameof(isCompleted));

            var interval = pollInterval ?? DefaultPollInterval;
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pollInterval),
                    "The poll interval must be positive."
                );
            }

            if (timeout.HasValue && timeout.Value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timeout),
                    "The timeout must be positive."
                );
            }

            using (
                var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken
                )
            )
            {
                if (timeout.HasValue)
                {
                    timeoutSource.CancelAfter(timeout.Value);
                }

                var token = timeoutSource.Token;
                try
                {
                    while (true)
                    {
                        var resource = await fetch(token).ConfigureAwait(false);
                        if (isCompleted(resource))
                        {
                            return resource;
                        }

                        await Task.Delay(interval, token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException exception)
                    when (timeout.HasValue && !cancellationToken.IsCancellationRequested)
                {
                    throw new LocawebServidoresTimeoutException(
                        "The operation did not complete within " + timeout.Value + ".",
                        exception
                    );
                }
            }
        }

        private static JsonApiResource<TAttributes> RequireData<TAttributes>(
            ResourceDocument<TAttributes> document
        )
        {
            return document.Data
                ?? throw new LocawebServidoresException(
                    "The API returned a document without primary data."
                );
        }
    }
}
