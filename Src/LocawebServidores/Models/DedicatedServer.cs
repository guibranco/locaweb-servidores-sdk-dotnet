namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a dedicated server (<c>/dedicated/servers</c>).
    /// </summary>
    public sealed class DedicatedServer : ServerAttributes
    {
        /// <summary>
        /// Gets or sets the data center / location of the server, when returned by the API.
        /// </summary>
        public string? Location { get; set; }
    }
}
