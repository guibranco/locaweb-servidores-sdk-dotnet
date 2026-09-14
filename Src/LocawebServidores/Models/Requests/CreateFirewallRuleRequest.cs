using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for creating a firewall rule on a cloud server.
    /// </summary>
    public sealed class CreateFirewallRuleRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFirewallRuleRequest"/> class.
        /// </summary>
        /// <param name="protocol">The protocol (see <see cref="FirewallProtocols"/>).</param>
        /// <param name="source">The source network in CIDR notation (for example <c>0.0.0.0/0</c>).</param>
        /// <param name="initialPort">The first port of the range; <c>null</c> for protocols without ports.</param>
        /// <param name="finalPort">The last port of the range; defaults to <paramref name="initialPort"/>.</param>
        public CreateFirewallRuleRequest(
            string protocol,
            string source,
            int? initialPort = null,
            int? finalPort = null
        )
        {
            Protocol = Guard.NotNullOrWhiteSpace(protocol, nameof(protocol));
            Source = Guard.NotNullOrWhiteSpace(source, nameof(source));
            InitialPort = initialPort;
            FinalPort = finalPort ?? initialPort;
        }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        /// Gets the source network in CIDR notation.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the first port of the range (serialized as <c>initial_port</c>).
        /// </summary>
        public int? InitialPort { get; }

        /// <summary>
        /// Gets the last port of the range (serialized as <c>final_port</c>).
        /// </summary>
        public int? FinalPort { get; }
    }
}
