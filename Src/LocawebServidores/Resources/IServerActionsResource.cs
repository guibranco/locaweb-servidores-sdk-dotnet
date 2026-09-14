using System;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Cloud server action endpoints (<c>/cloud/servers/{serverKey}/actions</c>).
    /// Actions (for example a reboot) run asynchronously on the server.
    /// </summary>
    public interface IServerActionsResource
    {
        /// <summary>
        /// Lists the actions of a cloud server. <c>GET /cloud/servers/{serverKey}/actions</c>
        /// </summary>
        Task<ResourceCollection<ServerAction>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of an action. <c>GET /cloud/servers/{serverKey}/actions/{actionId}</c>
        /// </summary>
        Task<JsonApiResource<ServerAction>> GetAsync(
            string serverKey,
            string actionId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates an action on a cloud server. <c>POST /cloud/servers/{serverKey}/actions</c>
        /// </summary>
        Task<JsonApiResource<ServerAction>> CreateAsync(
            string serverKey,
            CreateActionRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates an action of the given type on a cloud server.
        /// </summary>
        Task<JsonApiResource<ServerAction>> CreateAsync(
            string serverKey,
            string actionType,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Reboots a cloud server (creates a <see cref="ServerActionTypes.Reboot"/> action).
        /// </summary>
        Task<JsonApiResource<ServerAction>> RebootAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Polls an action until <paramref name="isCompleted"/> returns <c>true</c>.
        /// </summary>
        /// <param name="serverKey">The server key.</param>
        /// <param name="actionId">The action identifier.</param>
        /// <param name="isCompleted">Decides, from the latest action resource, whether polling should stop.</param>
        /// <param name="pollInterval">The delay between polls; defaults to <see cref="ApiResource.DefaultPollInterval"/>.</param>
        /// <param name="timeout">An optional overall limit; when exceeded a <see cref="Exceptions.LocawebServidoresTimeoutException"/> is thrown.</param>
        /// <param name="cancellationToken">A token to cancel the wait.</param>
        /// <returns>The action resource that satisfied <paramref name="isCompleted"/>.</returns>
        Task<JsonApiResource<ServerAction>> WaitForCompletionAsync(
            string serverKey,
            string actionId,
            Func<JsonApiResource<ServerAction>, bool> isCompleted,
            TimeSpan? pollInterval = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default
        );
    }
}
