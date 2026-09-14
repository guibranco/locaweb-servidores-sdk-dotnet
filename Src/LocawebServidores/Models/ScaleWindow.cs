using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a scalability window: a period during which the server runs on a
    /// larger plan.
    /// </summary>
    public sealed class ScaleWindow : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the plan the server scales to.
        /// </summary>
        public string? Plan { get; set; }

        /// <summary>
        /// Gets or sets when the window starts (serialized as <c>starts_at</c>).
        /// </summary>
        public DateTimeOffset? StartsAt { get; set; }

        /// <summary>
        /// Gets or sets when the window ends (serialized as <c>ends_at</c>).
        /// </summary>
        public DateTimeOffset? EndsAt { get; set; }

        /// <summary>
        /// Gets or sets the window status.
        /// </summary>
        public string? Status { get; set; }
    }
}
