using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a snapshot schedule ("agendamento de snapshot").
    /// </summary>
    public sealed class ScheduledSnapshot : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets how often the snapshot runs (for example <c>daily</c> or <c>weekly</c>).
        /// </summary>
        public string? Frequency { get; set; }

        /// <summary>
        /// Gets or sets the time of day the snapshot runs.
        /// </summary>
        public string? Time { get; set; }

        /// <summary>
        /// Gets or sets the day of the week for weekly schedules (serialized as <c>day_of_week</c>).
        /// </summary>
        public string? DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets how many snapshots are kept before the oldest is discarded.
        /// </summary>
        public int? Retention { get; set; }

        /// <summary>
        /// Gets or sets the schedule status.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets when the schedule was created (serialized as <c>created_at</c>).
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
