using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Default implementation of <see cref="IVpsServersResource"/>.
    /// </summary>
    public sealed class VpsServersResource : ApiResource, IVpsServersResource
    {
        private const string BasePath = "vps/servers";

        /// <summary>
        /// Initializes a new instance of the <see cref="VpsServersResource"/> class.
        /// </summary>
        public VpsServersResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<VpsServer>> ListAsync(
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<VpsServer>(BasePath, options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<JsonApiResource<VpsServer>> GetAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<VpsServer>(
                BasePath + "/" + Segment(serverKey, nameof(serverKey)),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ServerStatus> GetStatusAsync(
            string serverKey,
            CancellationToken cancellationToken = default
        )
        {
            var options = new RequestOptions().WithFields(
                ResourceTypes.Servers,
                "power_state",
                "status"
            );
            var resource = await GetAsync(serverKey, options, cancellationToken)
                .ConfigureAwait(false);
            return ServerStatus.FromResource(resource);
        }
    }
}
