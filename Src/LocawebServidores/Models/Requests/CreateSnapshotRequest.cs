using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for creating a snapshot of a cloud server.
    /// </summary>
    public sealed class CreateSnapshotRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSnapshotRequest"/> class.
        /// </summary>
        /// <param name="name">An optional snapshot name.</param>
        /// <param name="description">An optional description.</param>
        public CreateSnapshotRequest(string? name = null, string? description = null)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Gets the snapshot name, if any.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the snapshot description, if any.
        /// </summary>
        public string? Description { get; }
    }
}
