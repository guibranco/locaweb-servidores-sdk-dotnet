using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models.Requests
{
    /// <summary>
    /// Payload for changing a server's nickname ("apelido").
    /// </summary>
    public sealed class UpdateServerNicknameRequest : ResourceAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateServerNicknameRequest"/> class.
        /// </summary>
        /// <param name="nickname">The new nickname.</param>
        public UpdateServerNicknameRequest(string nickname)
        {
            Nickname = Guard.NotNullOrWhiteSpace(nickname, nameof(nickname));
        }

        /// <summary>
        /// Gets the new nickname.
        /// </summary>
        public string Nickname { get; }
    }
}
