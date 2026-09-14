using System;
using System.Net.Http;
using LocawebServidores.Http;
using LocawebServidores.Internal;
using LocawebServidores.Resources;

namespace LocawebServidores
{
    /// <summary>
    /// Client for the Locaweb Servidores API
    /// (<see href="https://developer.locaweb.com.br/docs/api-servidores"/>).
    /// </summary>
    /// <example>
    /// <code>
    /// using var client = new LocawebServidoresClient("your-api-token");
    /// var servers = await client.CloudServers.ListAsync();
    /// </code>
    /// </example>
    public sealed class LocawebServidoresClient : ILocawebServidoresClient, IDisposable
    {
        private readonly HttpClient? _ownedHttpClient;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance using the default options and the given API token.
        /// The client creates and owns its own <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="apiToken">The API token generated in the Locaweb servers panel.</param>
        public LocawebServidoresClient(string apiToken)
            : this(new LocawebServidoresClientOptions { ApiToken = apiToken }) { }

        /// <summary>
        /// Initializes a new instance with the given options. The client creates and owns
        /// its own <see cref="HttpClient"/>, applying <see cref="LocawebServidoresClientOptions.Timeout"/>.
        /// </summary>
        /// <param name="options">The client options.</param>
        public LocawebServidoresClient(LocawebServidoresClientOptions options)
            : this(CreateHttpClient(options), options, ownsHttpClient: true) { }

        /// <summary>
        /// Initializes a new instance using an externally managed <see cref="HttpClient"/>
        /// (for example one created by <c>IHttpClientFactory</c>). The client does not
        /// dispose it, and never mutates its default headers or base address.
        /// </summary>
        /// <param name="httpClient">The HTTP client to send requests with.</param>
        /// <param name="options">The client options.</param>
        public LocawebServidoresClient(
            HttpClient httpClient,
            LocawebServidoresClientOptions options
        )
            : this(httpClient, options, ownsHttpClient: false) { }

        /// <summary>
        /// Initializes a new instance on top of a custom <see cref="IApiConnection"/>.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        public LocawebServidoresClient(IApiConnection connection)
            : this(connection, null) { }

        private LocawebServidoresClient(
            HttpClient httpClient,
            LocawebServidoresClientOptions options,
            bool ownsHttpClient
        )
            : this(new ApiConnection(httpClient, options), ownsHttpClient ? httpClient : null) { }

        private LocawebServidoresClient(IApiConnection connection, HttpClient? ownedHttpClient)
        {
            Connection = Guard.NotNull(connection, nameof(connection));
            _ownedHttpClient = ownedHttpClient;

            CloudServers = new CloudServersResource(connection);
            VpsServers = new VpsServersResource(connection);
            DedicatedServers = new DedicatedServersResource(connection);
            Actions = new ServerActionsResource(connection);
            Ips = new IpsResource(connection);
            Firewalls = new FirewallsResource(connection);
            Snapshots = new SnapshotsResource(connection);
            ScheduledSnapshots = new ScheduledSnapshotsResource(connection);
            CustomTemplates = new CustomTemplatesResource(connection);
            Scalability = new ScalabilityResource(connection);
        }

        /// <inheritdoc />
        public IApiConnection Connection { get; }

        /// <inheritdoc />
        public ICloudServersResource CloudServers { get; }

        /// <inheritdoc />
        public IVpsServersResource VpsServers { get; }

        /// <inheritdoc />
        public IDedicatedServersResource DedicatedServers { get; }

        /// <inheritdoc />
        public IServerActionsResource Actions { get; }

        /// <inheritdoc />
        public IIpsResource Ips { get; }

        /// <inheritdoc />
        public IFirewallsResource Firewalls { get; }

        /// <inheritdoc />
        public ISnapshotsResource Snapshots { get; }

        /// <inheritdoc />
        public IScheduledSnapshotsResource ScheduledSnapshots { get; }

        /// <inheritdoc />
        public ICustomTemplatesResource CustomTemplates { get; }

        /// <inheritdoc />
        public IScalabilityResource Scalability { get; }

        /// <summary>
        /// Disposes the <see cref="HttpClient"/> created by this instance, if any.
        /// Externally supplied clients are left untouched.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _ownedHttpClient?.Dispose();
        }

        private static HttpClient CreateHttpClient(LocawebServidoresClientOptions options)
        {
            Guard.NotNull(options, nameof(options));
            options.Validate();
            return new HttpClient { Timeout = options.Timeout };
        }
    }
}
