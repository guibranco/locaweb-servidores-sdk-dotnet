namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a Cloud Server Pro instance (<c>/cloud/servers</c>).
    /// </summary>
    public sealed class CloudServer : ServerAttributes
    {
        /// <summary>
        /// Gets or sets the plan the server is currently on, when returned by the API.
        /// </summary>
        public string? Plan { get; set; }
    }
}
