using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of an IP address attached to a cloud server.
    /// </summary>
    public sealed class IpAddress : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the IP version (for example <c>4</c> or <c>6</c>), when returned by the API.
        /// </summary>
        public string? Version { get; set; }
    }
}
