using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// VPS endpoints (<c>/vps/servers</c>).
    /// </summary>
    public interface IVpsServersResource
    {
        /// <summary>
        /// Lists the VPS servers of the account. <c>GET /vps/servers</c>
        /// </summary>
        Task<ResourceCollection<VpsServer>> ListAsync(
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a VPS server. <c>GET /vps/servers/{serverKey}</c>
        /// </summary>
        Task<JsonApiResource<VpsServer>> GetAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the status, power state and available actions of a VPS server, requesting
        /// only those fields.
        /// </summary>
        Task<ServerStatus> GetStatusAsync(
            string serverKey,
            CancellationToken cancellationToken = default
        );
    }
}
