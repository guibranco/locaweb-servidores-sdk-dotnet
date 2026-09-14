using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of an operating system image available for (re)installing a cloud server.
    /// </summary>
    public sealed class InstallationImage : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the image name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the image description, when returned by the API.
        /// </summary>
        public string? Description { get; set; }
    }
}
