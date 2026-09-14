using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a cloud server snapshot.
    /// </summary>
    public sealed class Snapshot : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the snapshot name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the snapshot description, when returned by the API.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the snapshot status.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets when the snapshot was created (serialized as <c>created_at</c>).
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
