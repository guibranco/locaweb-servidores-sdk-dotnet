namespace LocawebServidores.Models
{
    /// <summary>
    /// Protocols accepted by firewall rules.
    /// </summary>
    public static class FirewallProtocols
    {
        /// <summary>TCP.</summary>
        public const string Tcp = "tcp";

        /// <summary>UDP.</summary>
        public const string Udp = "udp";

        /// <summary>ICMP.</summary>
        public const string Icmp = "icmp";
    }
}
