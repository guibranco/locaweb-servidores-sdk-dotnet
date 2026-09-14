using LocawebServidores.Http;
using LocawebServidores.Resources;

namespace LocawebServidores
{
    /// <summary>
    /// Entry point to the Locaweb Servidores API. Each property groups the endpoints of
    /// one API area; <see cref="Connection"/> gives raw access for anything not modeled.
    /// </summary>
    public interface ILocawebServidoresClient
    {
        /// <summary>
        /// Gets the low-level connection (authentication, serialization, error mapping).
        /// </summary>
        IApiConnection Connection { get; }

        /// <summary>
        /// Gets the Cloud Server Pro endpoints.
        /// </summary>
        ICloudServersResource CloudServers { get; }

        /// <summary>
        /// Gets the VPS endpoints.
        /// </summary>
        IVpsServersResource VpsServers { get; }

        /// <summary>
        /// Gets the dedicated server endpoints.
        /// </summary>
        IDedicatedServersResource DedicatedServers { get; }

        /// <summary>
        /// Gets the cloud server action endpoints (reboot and other asynchronous operations).
        /// </summary>
        IServerActionsResource Actions { get; }

        /// <summary>
        /// Gets the cloud server IP endpoints.
        /// </summary>
        IIpsResource Ips { get; }

        /// <summary>
        /// Gets the cloud server firewall endpoints.
        /// </summary>
        IFirewallsResource Firewalls { get; }

        /// <summary>
        /// Gets the cloud server snapshot endpoints.
        /// </summary>
        ISnapshotsResource Snapshots { get; }

        /// <summary>
        /// Gets the snapshot schedule endpoints.
        /// </summary>
        IScheduledSnapshotsResource ScheduledSnapshots { get; }

        /// <summary>
        /// Gets the custom template endpoints.
        /// </summary>
        ICustomTemplatesResource CustomTemplates { get; }

        /// <summary>
        /// Gets the scalability endpoints (scale windows, monitoring triggers, plans, status).
        /// </summary>
        IScalabilityResource Scalability { get; }
    }
}
