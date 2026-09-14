using System;
using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for creating or updating a scalability window.
    /// </summary>
    public sealed class ScaleWindowRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleWindowRequest"/> class.
        /// </summary>
        /// <param name="plan">The plan to scale to (see the scalability plans endpoint).</param>
        /// <param name="startsAt">When the window starts.</param>
        /// <param name="endsAt">When the window ends; must be after <paramref name="startsAt"/>.</param>
        public ScaleWindowRequest(string plan, DateTimeOffset startsAt, DateTimeOffset endsAt)
        {
            Plan = Guard.NotNullOrWhiteSpace(plan, nameof(plan));
            if (endsAt <= startsAt)
            {
                throw new ArgumentException("The window must end after it starts.", nameof(endsAt));
            }

            StartsAt = startsAt;
            EndsAt = endsAt;
        }

        /// <summary>
        /// Gets the plan to scale to.
        /// </summary>
        public string Plan { get; }

        /// <summary>
        /// Gets when the window starts (serialized as <c>starts_at</c>).
        /// </summary>
        public DateTimeOffset StartsAt { get; }

        /// <summary>
        /// Gets when the window ends (serialized as <c>ends_at</c>).
        /// </summary>
        public DateTimeOffset EndsAt { get; }
    }
}
