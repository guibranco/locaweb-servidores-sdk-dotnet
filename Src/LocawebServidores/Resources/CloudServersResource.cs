using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Default implementation of <see cref="ICloudServersResource"/>.
    /// </summary>
    public sealed class CloudServersResource : ApiResource, ICloudServersResource
    {
        private const string BasePath = "cloud/servers";

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudServersResource"/> class.
        /// </summary>
        public CloudServersResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<CloudServer>> ListAsync(
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<CloudServer>(BasePath, options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<JsonApiResource<CloudServer>> GetAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<CloudServer>(
                CloudServerPath(serverKey),
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
        public Task<JsonApiResource<CloudServer>> UpdateNicknameAsync(
            string serverKey,
            string nickname,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            var path = CloudServerPath(serverKey);
            return PutResourceAsync<CloudServer, UpdateServerNicknameRequest>(
                path,
                ResourceTypes.Servers,
                serverKey,
                new UpdateServerNicknameRequest(nickname),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<ResourceCollection<InstallationImage>> ListInstallationImagesAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<InstallationImage>(
                CloudServerPath(serverKey) + "/installation_images",
                options,
                cancellationToken
            );
        }
    }
}
