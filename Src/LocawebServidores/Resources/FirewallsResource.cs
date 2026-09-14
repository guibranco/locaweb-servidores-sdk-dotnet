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
    /// Default implementation of <see cref="IFirewallsResource"/>.
    /// </summary>
    public sealed class FirewallsResource : ApiResource, IFirewallsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallsResource"/> class.
        /// </summary>
        public FirewallsResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<FirewallRule>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<FirewallRule>(
                FirewallsPath(serverKey),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<FirewallRule>> GetAsync(
            string serverKey,
            string firewallId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<FirewallRule>(
                FirewallPath(serverKey, firewallId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<FirewallRule>> CreateAsync(
            string serverKey,
            CreateFirewallRuleRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PostResourceAsync<FirewallRule, CreateFirewallRuleRequest>(
                FirewallsPath(serverKey),
                ResourceTypes.Firewalls,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string serverKey,
            string firewallId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Connection.DeleteAsync(
                FirewallPath(serverKey, firewallId),
                options,
                cancellationToken
            );
        }

        private static string FirewallsPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/firewalls";
        }

        private static string FirewallPath(string serverKey, string firewallId)
        {
            return FirewallsPath(serverKey) + "/" + Segment(firewallId, nameof(firewallId));
        }
    }
}
