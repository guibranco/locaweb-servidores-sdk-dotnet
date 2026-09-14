using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LocawebServidores.Serialization
{
    /// <summary>
    /// Reads a JSON string, number, boolean or null into a <see cref="string"/>.
    /// Used for fields that the JSON:API specification defines as strings but that
    /// some servers emit as numbers (for example the <c>status</c> member of an error object).
    /// </summary>
    internal sealed class LenientStringConverter : JsonConverter<string?>
    {
        /// <inheritdoc />
        public override string? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var integer))
                    {
                        return integer.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    return reader
                        .GetDouble()
                        .ToString(System.Globalization.CultureInfo.InvariantCulture);
                case JsonTokenType.True:
                    return "true";
                case JsonTokenType.False:
                    return "false";
                default:
                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        return document.RootElement.GetRawText();
                    }
            }
        }

        /// <inheritdoc />
        public override void Write(
            Utf8JsonWriter writer,
            string? value,
            JsonSerializerOptions options
        )
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
