using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Cloud server firewall endpoints (<c>/cloud/servers/{serverKey}/firewalls</c>).
    /// </summary>
    public interface IFirewallsResource
    {
        /// <summary>
        /// Lists the firewall rules of a cloud server. <c>GET /cloud/servers/{serverKey}/firewalls</c>
        /// </summary>
        Task<ResourceCollection<FirewallRule>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a firewall rule. <c>GET /cloud/servers/{serverKey}/firewalls/{firewallId}</c>
        /// </summary>
        Task<JsonApiResource<FirewallRule>> GetAsync(
            string serverKey,
            string firewallId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates a firewall rule. <c>POST /cloud/servers/{serverKey}/firewalls</c>
        /// </summary>
        Task<JsonApiResource<FirewallRule>> CreateAsync(
            string serverKey,
            CreateFirewallRuleRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes a firewall rule. <c>DELETE /cloud/servers/{serverKey}/firewalls/{firewallId}</c>
        /// </summary>
        Task DeleteAsync(
            string serverKey,
            string firewallId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
