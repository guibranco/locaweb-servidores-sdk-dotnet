namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a VPS instance (<c>/vps/servers</c>).
    /// </summary>
    public sealed class VpsServer : ServerAttributes
    {
        /// <summary>
        /// Gets or sets the plan the server is currently on, when returned by the API.
        /// </summary>
        public string? Plan { get; set; }
    }
}
