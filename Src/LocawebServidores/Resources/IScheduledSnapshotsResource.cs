using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Snapshot schedule endpoints (<c>/cloud/servers/{serverKey}/scheduled_snapshots</c>).
    /// </summary>
    public interface IScheduledSnapshotsResource
    {
        /// <summary>
        /// Lists the snapshot schedules of a cloud server.
        /// <c>GET /cloud/servers/{serverKey}/scheduled_snapshots</c>
        /// </summary>
        Task<ResourceCollection<ScheduledSnapshot>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a snapshot schedule.
        /// <c>GET /cloud/servers/{serverKey}/scheduled_snapshots/{scheduledSnapshotId}</c>
        /// </summary>
        Task<JsonApiResource<ScheduledSnapshot>> GetAsync(
            string serverKey,
            string scheduledSnapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates a snapshot schedule. <c>POST /cloud/servers/{serverKey}/scheduled_snapshots</c>
        /// </summary>
        Task<JsonApiResource<ScheduledSnapshot>> CreateAsync(
            string serverKey,
            CreateScheduledSnapshotRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes a snapshot schedule.
        /// <c>DELETE /cloud/servers/{serverKey}/scheduled_snapshots/{scheduledSnapshotId}</c>
        /// </summary>
        Task DeleteAsync(
            string serverKey,
            string scheduledSnapshotId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
