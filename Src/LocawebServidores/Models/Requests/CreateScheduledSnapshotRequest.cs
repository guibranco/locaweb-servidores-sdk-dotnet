using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for creating a snapshot schedule on a cloud server.
    /// </summary>
    public sealed class CreateScheduledSnapshotRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateScheduledSnapshotRequest"/> class.
        /// </summary>
        /// <param name="frequency">How often the snapshot runs (for example <c>daily</c> or <c>weekly</c>).</param>
        /// <param name="time">The time of day the snapshot runs (for example <c>03:00</c>).</param>
        /// <param name="dayOfWeek">The day of the week, for weekly schedules.</param>
        /// <param name="retention">How many snapshots to keep.</param>
        public CreateScheduledSnapshotRequest(
            string frequency,
            string time,
            string? dayOfWeek = null,
            int? retention = null
        )
        {
            Frequency = Guard.NotNullOrWhiteSpace(frequency, nameof(frequency));
            Time = Guard.NotNullOrWhiteSpace(time, nameof(time));
            DayOfWeek = dayOfWeek;
            Retention = retention;
        }

        /// <summary>
        /// Gets how often the snapshot runs.
        /// </summary>
        public string Frequency { get; }

        /// <summary>
        /// Gets the time of day the snapshot runs.
        /// </summary>
        public string Time { get; }

        /// <summary>
        /// Gets the day of the week for weekly schedules (serialized as <c>day_of_week</c>).
        /// </summary>
        public string? DayOfWeek { get; }

        /// <summary>
        /// Gets how many snapshots to keep.
        /// </summary>
        public int? Retention { get; }
    }
}
