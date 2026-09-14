using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Cloud Server Pro endpoints (<c>/cloud/servers</c>).
    /// </summary>
    public interface ICloudServersResource
    {
        /// <summary>
        /// Lists the cloud servers of the account. <c>GET /cloud/servers</c>
        /// </summary>
        Task<ResourceCollection<CloudServer>> ListAsync(
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a cloud server. <c>GET /cloud/servers/{serverKey}</c>
        /// </summary>
        Task<JsonApiResource<CloudServer>> GetAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the status, power state and available actions of a cloud server, requesting
        /// only those fields. <c>GET /cloud/servers/{serverKey}?fields[servers]=power_state,status</c>
        /// </summary>
        Task<ServerStatus> GetStatusAsync(
            string serverKey,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Changes the nickname of a cloud server. <c>PUT /cloud/servers/{serverKey}</c>
        /// </summary>
        Task<JsonApiResource<CloudServer>> UpdateNicknameAsync(
            string serverKey,
            string nickname,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lists the operating system images available for (re)installing the server.
        /// <c>GET /cloud/servers/{serverKey}/installation_images</c>
        /// </summary>
        Task<ResourceCollection<InstallationImage>> ListInstallationImagesAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
