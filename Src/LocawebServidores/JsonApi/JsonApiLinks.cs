using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// A JSON:API links object. Each member is either a URL string or a link object
    /// with an <c>href</c> member; <see cref="GetHref(string)"/> handles both.
    /// </summary>
    public sealed class JsonApiLinks : Dictionary<string, JsonElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiLinks"/> class.
        /// </summary>
        public JsonApiLinks()
            : base(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// Gets the <c>self</c> link, if any.
        /// </summary>
        public string? Self => GetHref("self");

        /// <summary>
        /// Gets the <c>related</c> link, if any.
        /// </summary>
        public string? Related => GetHref("related");

        /// <summary>
        /// Gets the <c>first</c> pagination link, if any.
        /// </summary>
        public string? First => GetHref("first");

        /// <summary>
        /// Gets the <c>last</c> pagination link, if any.
        /// </summary>
        public string? Last => GetHref("last");

        /// <summary>
        /// Gets the <c>prev</c> pagination link, if any.
        /// </summary>
        public string? Prev => GetHref("prev");

        /// <summary>
        /// Gets the <c>next</c> pagination link, if any.
        /// </summary>
        public string? Next => GetHref("next");

        /// <summary>
        /// Resolves the URL of a named link, whether it is expressed as a plain string
        /// or as a link object with an <c>href</c> member.
        /// </summary>
        /// <param name="name">The link name (for example <c>next</c>).</param>
        /// <returns>The URL, or <c>null</c> when the link is absent or null.</returns>
        public string? GetHref(string name)
        {
            if (string.IsNullOrEmpty(name) || !TryGetValue(name, out var element))
            {
                return null;
            }

            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Object:
                    return
                        element.TryGetProperty("href", out var href)
                        && href.ValueKind == JsonValueKind.String
                        ? href.GetString()
                        : null;
                default:
                    return null;
            }
        }
    }
}
