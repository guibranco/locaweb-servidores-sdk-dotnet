using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a firewall rule of a cloud server.
    /// </summary>
    public sealed class FirewallRule : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the protocol (see <see cref="FirewallProtocols"/>).
        /// </summary>
        public string? Protocol { get; set; }

        /// <summary>
        /// Gets or sets the first port of the range (serialized as <c>initial_port</c>).
        /// </summary>
        public int? InitialPort { get; set; }

        /// <summary>
        /// Gets or sets the last port of the range (serialized as <c>final_port</c>).
        /// </summary>
        public int? FinalPort { get; set; }

        /// <summary>
        /// Gets or sets the source network in CIDR notation (for example <c>200.200.200.0/24</c>).
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets when the rule was created (serialized as <c>created_at</c>).
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
