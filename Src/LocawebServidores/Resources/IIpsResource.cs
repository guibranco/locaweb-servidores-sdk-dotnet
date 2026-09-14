using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Cloud server IP endpoints (<c>/cloud/servers/{serverKey}/ips</c>).
    /// </summary>
    public interface IIpsResource
    {
        /// <summary>
        /// Lists the IP addresses of a cloud server. <c>GET /cloud/servers/{serverKey}/ips</c>
        /// </summary>
        Task<ResourceCollection<IpAddress>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of an IP address. <c>GET /cloud/servers/{serverKey}/ips/{ipId}</c>
        /// </summary>
        Task<JsonApiResource<IpAddress>> GetAsync(
            string serverKey,
            string ipId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
