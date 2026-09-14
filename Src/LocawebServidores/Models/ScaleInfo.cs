using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes describing the scalability status of a cloud server.
    /// </summary>
    public sealed class ScaleInfo : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the scalability status.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the plan the server is currently running on.
        /// </summary>
        public string? Plan { get; set; }
    }
}
