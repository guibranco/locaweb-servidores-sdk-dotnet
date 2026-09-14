using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Default implementation of <see cref="IDedicatedServersResource"/>.
    /// </summary>
    public sealed class DedicatedServersResource : ApiResource, IDedicatedServersResource
    {
        private const string BasePath = "dedicated/servers";

        /// <summary>
        /// Initializes a new instance of the <see cref="DedicatedServersResource"/> class.
        /// </summary>
        public DedicatedServersResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<DedicatedServer>> ListAsync(
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<DedicatedServer>(BasePath, options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<JsonApiResource<DedicatedServer>> GetAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<DedicatedServer>(
                ServerPath(serverKey),
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

        /// <inheritdoc />
        public Task<JsonApiResource<DedicatedServer>> UpdateNicknameAsync(
            string serverKey,
            string nickname,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            var path = ServerPath(serverKey);
            return PutResourceAsync<DedicatedServer, UpdateServerNicknameRequest>(
                path,
                ResourceTypes.Servers,
                serverKey,
                new UpdateServerNicknameRequest(nickname),
                options,
                cancellationToken
            );
        }

        private static string ServerPath(string serverKey)
        {
            return BasePath + "/" + Segment(serverKey, nameof(serverKey));
        }
    }
}
