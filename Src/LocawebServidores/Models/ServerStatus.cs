using System;
using System.Collections.Generic;
using System.Text.Json;
using LocawebServidores.Internal;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Models
{
    /// <summary>
    /// A lightweight monitoring view of a server: its status, power state and the
    /// actions the API currently allows (taken from <c>meta.actions</c>).
    /// </summary>
    public sealed class ServerStatus
    {
        private const string ActionsMetaName = "actions";
        private static readonly string[] ActionNameMembers = { "type", "name", "action" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerStatus"/> class.
        /// </summary>
        public ServerStatus(
            string? id,
            string? status,
            string? powerState,
            IReadOnlyList<string>? availableActions
        )
        {
            Id = id;
            Status = status;
            PowerState = powerState;
            AvailableActions = availableActions ?? Array.Empty<string>();
        }

        /// <summary>
        /// Gets the server key.
        /// </summary>
        public string? Id { get; }

        /// <summary>
        /// Gets the provisioning/business status reported by the API.
        /// </summary>
        public string? Status { get; }

        /// <summary>
        /// Gets the power state reported by the API.
        /// </summary>
        public string? PowerState { get; }

        /// <summary>
        /// Gets the action types currently available for the server, when the API
        /// lists them under <c>meta.actions</c>. Empty otherwise.
        /// </summary>
        public IReadOnlyList<string> AvailableActions { get; }

        /// <summary>
        /// Builds a <see cref="ServerStatus"/> from a server resource, reading the
        /// available actions from <c>meta.actions</c> (either strings or objects with a
        /// <c>type</c>/<c>name</c> member).
        /// </summary>
        /// <typeparam name="TServer">The server attributes type.</typeparam>
        /// <param name="resource">The server resource.</param>
        /// <returns>The status view.</returns>
        public static ServerStatus FromResource<TServer>(JsonApiResource<TServer> resource)
            where TServer : ServerAttributes
        {
            Guard.NotNull(resource, nameof(resource));

            List<string>? actions = null;
            if (
                resource.TryGetMeta(ActionsMetaName, out var element)
                && element.ValueKind == JsonValueKind.Array
            )
            {
                actions = new List<string>();
                foreach (var item in element.EnumerateArray())
                {
                    var name = ReadActionName(item);
                    if (!string.IsNullOrEmpty(name))
                    {
                        actions.Add(name!);
                    }
                }
            }

            return new ServerStatus(
                resource.Id,
                resource.Attributes?.Status,
                resource.Attributes?.PowerState,
                actions
            );
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return (Id ?? "?")
                + ": status="
                + (Status ?? "?")
                + ", power_state="
                + (PowerState ?? "?");
        }

        private static string? ReadActionName(JsonElement item)
        {
            if (item.ValueKind == JsonValueKind.String)
            {
                return item.GetString();
            }

            if (item.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            foreach (var member in ActionNameMembers)
            {
                if (
                    item.TryGetProperty(member, out var value)
                    && value.ValueKind == JsonValueKind.String
                )
                {
                    return value.GetString();
                }
            }

            return null;
        }
    }
}
