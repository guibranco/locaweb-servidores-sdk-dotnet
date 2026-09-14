using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// Attributes of a plan a cloud server can scale to.
    /// </summary>
    public sealed class ScalabilityPlan : ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the plan name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the number of virtual CPUs.
        /// </summary>
        public int? Cpu { get; set; }

        /// <summary>
        /// Gets or sets the amount of memory.
        /// </summary>
        public int? Memory { get; set; }
    }
}
