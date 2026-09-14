using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes shared by cloud, VPS and dedicated servers. The API exposes many
    /// more attributes (plan, operating system, resources, and so on); they are available
    /// through <see cref="ResourceAttributes.AdditionalAttributes"/> and
    /// <see cref="ResourceAttributes.GetAttribute{T}(string)"/>.
    /// </summary>
    public abstract class ServerAttributes : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the server name (hostname / server key).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the user-defined nickname ("apelido").
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// Gets or sets the provisioning/business status of the server.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the power state of the server (serialized as <c>power_state</c>).
        /// </summary>
        public string? PowerState { get; set; }

        /// <summary>
        /// Gets or sets when the server was created (serialized as <c>created_at</c>).
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the server was last updated (serialized as <c>updated_at</c>).
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
