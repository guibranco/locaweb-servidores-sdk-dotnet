using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using LocawebServidores.Serialization;

namespace LocawebServidores.JsonApi
{
    /// <summary>
    /// Base class for typed attribute models. Any attribute the API returns that has
    /// no dedicated property is preserved in <see cref="AdditionalAttributes"/>, and any
    /// entry added there is sent to the API when the model is used in a request.
    /// </summary>
    public abstract class ResourceAttributes
    {
        /// <summary>
        /// Gets or sets the attributes not mapped to a dedicated property, keyed by their
        /// JSON name (snake_case).
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JsonElement>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Tries to read an attribute from <see cref="AdditionalAttributes"/>.
        /// </summary>
        /// <param name="name">The JSON attribute name.</param>
        /// <param name="value">The raw value when found.</param>
        /// <returns><c>true</c> when the attribute exists.</returns>
        public bool TryGetAttribute(string name, out JsonElement value)
        {
            if (
                AdditionalAttributes != null
                && !string.IsNullOrEmpty(name)
                && AdditionalAttributes.TryGetValue(name, out value)
            )
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Reads an attribute from <see cref="AdditionalAttributes"/> and deserializes it.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="name">The JSON attribute name.</param>
        /// <returns>The value, or <c>default</c> when the attribute is absent or null.</returns>
        public T? GetAttribute<T>(string name)
        {
            if (!TryGetAttribute(name, out var element) || element.ValueKind == JsonValueKind.Null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(
                element.GetRawText(),
                LocawebServidoresJson.Default
            );
        }

        /// <summary>
        /// Sets an attribute in <see cref="AdditionalAttributes"/>, serializing the value
        /// with the SDK settings. Useful for sending attributes this SDK does not model.
        /// </summary>
        /// <param name="name">The JSON attribute name.</param>
        /// <param name="value">The value to send; <c>null</c> sends a JSON null.</param>
        public void SetAttribute(string name, object? value)
        {
            AdditionalAttributes ??= new Dictionary<string, JsonElement>();
            var json = JsonSerializer.Serialize(value, LocawebServidoresJson.Default);
            using (var document = JsonDocument.Parse(json))
            {
                AdditionalAttributes[name] = document.RootElement.Clone();
            }
        }
    }
}
