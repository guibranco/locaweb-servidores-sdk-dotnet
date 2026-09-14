using System;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for updating a scalability monitoring trigger. Only the values you set
    /// are sent.
    /// </summary>
    public sealed class UpdateEventTriggerRequest : ResourceAttributes
    {
        private int? _threshold;

        /// <summary>
        /// Gets or sets the usage percentage (0-100) that fires the trigger.
        /// </summary>
        public int? Threshold
        {
            get => _threshold;
            set
            {
                if (value.HasValue && (value.Value < 0 || value.Value > 100))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "The threshold must be a percentage between 0 and 100."
                    );
                }

                _threshold = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the trigger is active.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or sets the plan to scale to when the trigger fires, if applicable.
        /// </summary>
        public string? Plan { get; set; }
    }
}
