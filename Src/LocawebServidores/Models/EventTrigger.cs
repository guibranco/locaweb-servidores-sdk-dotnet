using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a scalability monitoring trigger: the API alerts (and can scale)
    /// when CPU or memory consumption reaches the configured percentage.
    /// </summary>
    public sealed class EventTrigger : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the monitored metric (for example <c>cpu</c> or <c>memory</c>).
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the usage percentage that fires the trigger.
        /// </summary>
        public int? Threshold { get; set; }

        /// <summary>
        /// Gets or sets whether the trigger is active.
        /// </summary>
        public bool? Enabled { get; set; }
    }
}
