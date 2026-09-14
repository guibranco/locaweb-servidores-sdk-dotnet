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
    /// Default implementation of <see cref="IScheduledSnapshotsResource"/>.
    /// </summary>
    public sealed class ScheduledSnapshotsResource : ApiResource, IScheduledSnapshotsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledSnapshotsResource"/> class.
        /// </summary>
        public ScheduledSnapshotsResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<ScheduledSnapshot>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<ScheduledSnapshot>(
                SchedulesPath(serverKey),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ScheduledSnapshot>> GetAsync(
            string serverKey,
            string scheduledSnapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<ScheduledSnapshot>(
                SchedulePath(serverKey, scheduledSnapshotId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ScheduledSnapshot>> CreateAsync(
            string serverKey,
            CreateScheduledSnapshotRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PostResourceAsync<ScheduledSnapshot, CreateScheduledSnapshotRequest>(
                SchedulesPath(serverKey),
                ResourceTypes.ScheduledSnapshots,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string serverKey,
            string scheduledSnapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Connection.DeleteAsync(
                SchedulePath(serverKey, scheduledSnapshotId),
                options,
                cancellationToken
            );
        }

        private static string SchedulesPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/scheduled_snapshots";
        }

        private static string SchedulePath(string serverKey, string scheduledSnapshotId)
        {
            return SchedulesPath(serverKey)
                + "/"
                + Segment(scheduledSnapshotId, nameof(scheduledSnapshotId));
        }
    }
}
