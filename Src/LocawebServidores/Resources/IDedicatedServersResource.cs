using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Dedicated server endpoints (<c>/dedicated/servers</c>).
    /// </summary>
    public interface IDedicatedServersResource
    {
        /// <summary>
        /// Lists the dedicated servers of the account. <c>GET /dedicated/servers</c>
        /// </summary>
        Task<ResourceCollection<DedicatedServer>> ListAsync(
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a dedicated server. <c>GET /dedicated/servers/{serverKey}</c>
        /// </summary>
        Task<JsonApiResource<DedicatedServer>> GetAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the status, power state and available actions of a dedicated server,
        /// requesting only those fields.
        /// </summary>
        Task<ServerStatus> GetStatusAsync(
            string serverKey,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Changes the nickname of a dedicated server. <c>PUT /dedicated/servers/{serverKey}</c>
        /// </summary>
        Task<JsonApiResource<DedicatedServer>> UpdateNicknameAsync(
            string serverKey,
            string nickname,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
