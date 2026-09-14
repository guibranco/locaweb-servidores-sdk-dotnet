namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// The JSON:API resource type names used by the Locaweb Servidores API.
    /// </summary>
    public static class ResourceTypes
    {
        /// <summary>Cloud, VPS and dedicated servers.</summary>
        public const string Servers = "servers";

        /// <summary>Actions executed against a server or a snapshot (for example a reboot).</summary>
        public const string Actions = "actions";

        /// <summary>Operating system images available for (re)installation.</summary>
        public const string InstallationImages = "installation_images";

        /// <summary>IP addresses attached to a server.</summary>
        public const string Ips = "ips";

        /// <summary>Firewall rules.</summary>
        public const string Firewalls = "firewalls";

        /// <summary>Snapshots.</summary>
        public const string Snapshots = "snapshots";

        /// <summary>Snapshot schedules.</summary>
        public const string ScheduledSnapshots = "scheduled_snapshots";

        /// <summary>Custom templates created from a server.</summary>
        public const string CustomTemplates = "custom_templates";

        /// <summary>Scalability windows.</summary>
        public const string ScaleWindows = "scale_windows";

        /// <summary>Scalability monitoring triggers.</summary>
        public const string EventTriggers = "event_triggers";

        /// <summary>Plans a server can scale to.</summary>
        public const string Plans = "plans";

        /// <summary>Scalability status of a server.</summary>
        public const string ScaleInfo = "scale_info";
    }
}
