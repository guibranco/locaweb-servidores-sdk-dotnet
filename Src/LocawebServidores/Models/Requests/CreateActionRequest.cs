using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for creating an action on a server or a snapshot.
    /// </summary>
    public sealed class CreateActionRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateActionRequest"/> class.
        /// </summary>
        /// <param name="type">The action type, for example <see cref="ServerActionTypes.Reboot"/>.</param>
        public CreateActionRequest(string type)
        {
            Type = Guard.NotNullOrWhiteSpace(type, nameof(type));
        }

        /// <summary>
        /// Gets the action type.
        /// </summary>
        public string Type { get; }
    }
}
