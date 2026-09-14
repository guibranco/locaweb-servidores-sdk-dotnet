using System;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Cloud server snapshot endpoints (<c>/cloud/servers/{serverKey}/snapshots</c>),
    /// including the actions that can be run on a snapshot (for example restoring it).
    /// </summary>
    public interface ISnapshotsResource
    {
        /// <summary>
        /// Lists the snapshots of a cloud server. <c>GET /cloud/servers/{serverKey}/snapshots</c>
        /// </summary>
        Task<ResourceCollection<Snapshot>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a snapshot. <c>GET /cloud/servers/{serverKey}/snapshots/{snapshotId}</c>
        /// </summary>
        Task<JsonApiResource<Snapshot>> GetAsync(
            string serverKey,
            string snapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates a snapshot of a cloud server. <c>POST /cloud/servers/{serverKey}/snapshots</c>
        /// </summary>
        Task<JsonApiResource<Snapshot>> CreateAsync(
            string serverKey,
            CreateSnapshotRequest? request = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes a snapshot. <c>DELETE /cloud/servers/{serverKey}/snapshots/{snapshotId}</c>
        /// </summary>
        Task DeleteAsync(
            string serverKey,
            string snapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lists the actions of a snapshot. <c>GET /cloud/servers/{serverKey}/snapshots/{snapshotId}/actions</c>
        /// </summary>
        Task<ResourceCollection<ServerAction>> ListActionsAsync(
            string serverKey,
            string snapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a snapshot action.
        /// <c>GET /cloud/servers/{serverKey}/snapshots/{snapshotId}/actions/{actionId}</c>
        /// </summary>
        Task<JsonApiResource<ServerAction>> GetActionAsync(
            string serverKey,
            string snapshotId,
            string actionId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates an action on a snapshot. <c>POST /cloud/servers/{serverKey}/snapshots/{snapshotId}/actions</c>
        /// </summary>
        Task<JsonApiResource<ServerAction>> CreateActionAsync(
            string serverKey,
            string snapshotId,
            CreateActionRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates an action of the given type on a snapshot.
        /// </summary>
        Task<JsonApiResource<ServerAction>> CreateActionAsync(
            string serverKey,
            string snapshotId,
            string actionType,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Polls a snapshot action until <paramref name="isCompleted"/> returns <c>true</c>.
        /// </summary>
        Task<JsonApiResource<ServerAction>> WaitForActionCompletionAsync(
            string serverKey,
            string snapshotId,
            string actionId,
            Func<JsonApiResource<ServerAction>, bool> isCompleted,
            TimeSpan? pollInterval = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default
        );
    }
}
