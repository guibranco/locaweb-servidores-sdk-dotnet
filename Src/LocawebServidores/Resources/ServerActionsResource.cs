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
    /// Default implementation of <see cref="IServerActionsResource"/>.
    /// </summary>
    public sealed class ServerActionsResource : ApiResource, IServerActionsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerActionsResource"/> class.
        /// </summary>
        public ServerActionsResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<ServerAction>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<ServerAction>(
                ActionsPath(serverKey),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> GetAsync(
            string serverKey,
            string actionId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<ServerAction>(
                ActionsPath(serverKey) + "/" + Segment(actionId, nameof(actionId)),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> CreateAsync(
            string serverKey,
            CreateActionRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PostResourceAsync<ServerAction, CreateActionRequest>(
                ActionsPath(serverKey),
                ResourceTypes.Actions,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> CreateAsync(
            string serverKey,
            string actionType,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return CreateAsync(
                serverKey,
                new CreateActionRequest(actionType),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> RebootAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return CreateAsync(serverKey, ServerActionTypes.Reboot, options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<JsonApiResource<ServerAction>> WaitForCompletionAsync(
            string serverKey,
            string actionId,
            Func<JsonApiResource<ServerAction>, bool> isCompleted,
            TimeSpan? pollInterval = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNullOrWhiteSpace(serverKey, nameof(serverKey));
            Guard.NotNullOrWhiteSpace(actionId, nameof(actionId));
            return PollUntilAsync(
                token => GetAsync(serverKey, actionId, null, token),
                isCompleted,
                pollInterval,
                timeout,
                cancellationToken
            );
        }

        private static string ActionsPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/actions";
        }
    }
}
