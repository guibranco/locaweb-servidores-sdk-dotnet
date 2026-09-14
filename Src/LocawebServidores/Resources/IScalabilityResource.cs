using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Scalability endpoints (<c>/cloud/servers/{serverKey}/scalability/...</c>): scale
    /// windows, monitoring triggers, compatible plans and the current scalability status.
    /// </summary>
    public interface IScalabilityResource
    {
        /// <summary>
        /// Lists the scale windows of a cloud server.
        /// <c>GET /cloud/servers/{serverKey}/scalability/scale_windows</c>
        /// </summary>
        Task<ResourceCollection<ScaleWindow>> ListScaleWindowsAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates a scale window. <c>POST /cloud/servers/{serverKey}/scalability/scale_windows</c>
        /// </summary>
        Task<JsonApiResource<ScaleWindow>> CreateScaleWindowAsync(
            string serverKey,
            ScaleWindowRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Updates a scale window.
        /// <c>PUT /cloud/servers/{serverKey}/scalability/scale_windows/{scaleWindowId}</c>
        /// </summary>
        Task<JsonApiResource<ScaleWindow>> UpdateScaleWindowAsync(
            string serverKey,
            string scaleWindowId,
            ScaleWindowRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes a scale window.
        /// <c>DELETE /cloud/servers/{serverKey}/scalability/scale_windows/{scaleWindowId}</c>
        /// </summary>
        Task DeleteScaleWindowAsync(
            string serverKey,
            string scaleWindowId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Stops a scale window that is in progress.
        /// <c>POST /cloud/servers/{serverKey}/scalability/scale_windows/{scaleWindowId}/stop</c>
        /// </summary>
        Task StopScaleWindowAsync(
            string serverKey,
            string scaleWindowId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lists the monitoring triggers of a cloud server.
        /// <c>GET /cloud/servers/{serverKey}/scalability/event_triggers</c>
        /// </summary>
        Task<ResourceCollection<EventTrigger>> ListEventTriggersAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Updates a monitoring trigger.
        /// <c>PUT /cloud/servers/{serverKey}/scalability/event_triggers/{eventTriggerId}</c>
        /// </summary>
        Task<JsonApiResource<EventTrigger>> UpdateEventTriggerAsync(
            string serverKey,
            string eventTriggerId,
            UpdateEventTriggerRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lists the plans the server can scale to.
        /// <c>GET /cloud/servers/{serverKey}/scalability/plans</c>
        /// </summary>
        Task<ResourceCollection<ScalabilityPlan>> ListPlansAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the scalability status of a cloud server.
        /// <c>GET /cloud/servers/{serverKey}/scalability/scale_info</c>
        /// </summary>
        Task<JsonApiResource<ScaleInfo>> GetScaleInfoAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
