using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for creating a custom template from a cloud server.
    /// </summary>
    public sealed class CreateCustomTemplateRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCustomTemplateRequest"/> class.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="description">An optional description.</param>
        public CreateCustomTemplateRequest(string name, string? description = null)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
            Description = description;
        }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the template description, if any.
        /// </summary>
        public string? Description { get; }
    }
}
