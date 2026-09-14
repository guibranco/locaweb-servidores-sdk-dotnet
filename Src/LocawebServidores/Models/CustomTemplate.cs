using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a custom template created from a cloud server.
    /// </summary>
    public sealed class CustomTemplate : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the template name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the template description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the template status.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets when the template was created (serialized as <c>created_at</c>).
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
