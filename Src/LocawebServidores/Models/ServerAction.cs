using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of an action executed against a server or a snapshot
    /// (for example a reboot). Actions run asynchronously; poll
    /// <see cref="Status"/> or use the <c>WaitForCompletionAsync</c> helpers.
    /// </summary>
    public sealed class ServerAction : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the action type (for example <see cref="ServerActionTypes.Reboot"/>).
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the execution status of the action.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets when the action was created (serialized as <c>created_at</c>).
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the action was last updated (serialized as <c>updated_at</c>).
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
