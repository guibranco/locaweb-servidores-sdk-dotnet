using System;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.Internal;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Default implementation of <see cref="ISnapshotsResource"/>.
    /// </summary>
    public sealed class SnapshotsResource : ApiResource, ISnapshotsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotsResource"/> class.
        /// </summary>
        public SnapshotsResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<Snapshot>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<Snapshot>(
                SnapshotsPath(serverKey),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<Snapshot>> GetAsync(
            string serverKey,
            string snapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<Snapshot>(
                SnapshotPath(serverKey, snapshotId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<Snapshot>> CreateAsync(
            string serverKey,
            CreateSnapshotRequest? request = null,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return PostResourceAsync<Snapshot, CreateSnapshotRequest>(
                SnapshotsPath(serverKey),
                ResourceTypes.Snapshots,
                request ?? new CreateSnapshotRequest(),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string serverKey,
            string snapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Connection.DeleteAsync(
                SnapshotPath(serverKey, snapshotId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<ResourceCollection<ServerAction>> ListActionsAsync(
            string serverKey,
            string snapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<ServerAction>(
                ActionsPath(serverKey, snapshotId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> GetActionAsync(
            string serverKey,
            string snapshotId,
            string actionId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<ServerAction>(
                ActionsPath(serverKey, snapshotId) + "/" + Segment(actionId, nameof(actionId)),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> CreateActionAsync(
            string serverKey,
            string snapshotId,
            CreateActionRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PostResourceAsync<ServerAction, CreateActionRequest>(
                ActionsPath(serverKey, snapshotId),
                ResourceTypes.Actions,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> CreateActionAsync(
            string serverKey,
            string snapshotId,
            string actionType,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return CreateActionAsync(
                serverKey,
                snapshotId,
                new CreateActionRequest(actionType),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> WaitForActionCompletionAsync(
            string serverKey,
            string snapshotId,
            string actionId,
            Func<JsonApiResource<ServerAction>, bool> isCompleted,
            TimeSpan? pollInterval = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNullOrWhiteSpace(serverKey, nameof(serverKey));
            Guard.NotNullOrWhiteSpace(snapshotId, nameof(snapshotId));
            Guard.NotNullOrWhiteSpace(actionId, nameof(actionId));
            return PollUntilAsync(
                token => GetActionAsync(serverKey, snapshotId, actionId, null, token),
                isCompleted,
                pollInterval,
                timeout,
                cancellationToken
            );
        }

        private static string SnapshotsPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/snapshots";
        }

        private static string SnapshotPath(string serverKey, string snapshotId)
        {
            return SnapshotsPath(serverKey) + "/" + Segment(snapshotId, nameof(snapshotId));
        }

        private static string ActionsPath(string serverKey, string snapshotId)
        {
            return SnapshotPath(serverKey, snapshotId) + "/actions";
        }
    }
}
