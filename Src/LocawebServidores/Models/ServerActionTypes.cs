namespace LocawebServidores.Models
{
    /// <summary>
    /// Well-known action types accepted by the actions endpoints. The full list of
    /// actions available for a given server is returned by the API under
    /// <c>meta.actions</c> of the server resource (see
    /// <see cref="ServerStatus.AvailableActions"/>).
    /// </summary>
    public static class ServerActionTypes
    {
        /// <summary>
        /// Restarts the server.
        /// </summary>
        public const string Reboot = "reboot";
    }
}
