using System.Net.Http;
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
    /// Default implementation of <see cref="IScalabilityResource"/>.
    /// </summary>
    public sealed class ScalabilityResource : ApiResource, IScalabilityResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalabilityResource"/> class.
        /// </summary>
        public ScalabilityResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<ScaleWindow>> ListScaleWindowsAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<ScaleWindow>(
                ScaleWindowsPath(serverKey),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ScaleWindow>> CreateScaleWindowAsync(
            string serverKey,
            ScaleWindowRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PostResourceAsync<ScaleWindow, ScaleWindowRequest>(
                ScaleWindowsPath(serverKey),
                ResourceTypes.ScaleWindows,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ScaleWindow>> UpdateScaleWindowAsync(
            string serverKey,
            string scaleWindowId,
            ScaleWindowRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PutResourceAsync<ScaleWindow, ScaleWindowRequest>(
                ScaleWindowPath(serverKey, scaleWindowId),
                ResourceTypes.ScaleWindows,
                scaleWindowId,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task DeleteScaleWindowAsync(
            string serverKey,
            string scaleWindowId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Connection.DeleteAsync(
                ScaleWindowPath(serverKey, scaleWindowId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task StopScaleWindowAsync(
            string serverKey,
            string scaleWindowId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Connection.SendAsync(
                HttpMethod.Post,
                ScaleWindowPath(serverKey, scaleWindowId) + "/stop",
                null,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<ResourceCollection<EventTrigger>> ListEventTriggersAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<EventTrigger>(
                ScalabilityPath(serverKey) + "/event_triggers",
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<EventTrigger>> UpdateEventTriggerAsync(
            string serverKey,
            string eventTriggerId,
            UpdateEventTriggerRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PutResourceAsync<EventTrigger, UpdateEventTriggerRequest>(
                ScalabilityPath(serverKey)
                    + "/event_triggers/"
                    + Segment(eventTriggerId, nameof(eventTriggerId)),
                ResourceTypes.EventTriggers,
                eventTriggerId,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<ResourceCollection<ScalabilityPlan>> ListPlansAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<ScalabilityPlan>(
                ScalabilityPath(serverKey) + "/plans",
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ScaleInfo>> GetScaleInfoAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<ScaleInfo>(
                ScalabilityPath(serverKey) + "/scale_info",
                options,
                cancellationToken
            );
        }

        private static string ScalabilityPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/scalability";
        }

        private static string ScaleWindowsPath(string serverKey)
        {
            return ScalabilityPath(serverKey) + "/scale_windows";
        }

        private static string ScaleWindowPath(string serverKey, string scaleWindowId)
        {
            return ScaleWindowsPath(serverKey)
                + "/"
                + Segment(scaleWindowId, nameof(scaleWindowId));
        }
    }
}
