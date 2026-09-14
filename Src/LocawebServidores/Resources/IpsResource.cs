using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Default implementation of <see cref="IIpsResource"/>.
    /// </summary>
    public sealed class IpsResource : ApiResource, IIpsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IpsResource"/> class.
        /// </summary>
        public IpsResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<IpAddress>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<IpAddress>(IpsPath(serverKey), options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<JsonApiResource<IpAddress>> GetAsync(
            string serverKey,
            string ipId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<IpAddress>(
                IpsPath(serverKey) + "/" + Segment(ipId, nameof(ipId)),
                options,
                cancellationToken
            );
        }

        private static string IpsPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/ips";
        }
    }
}
