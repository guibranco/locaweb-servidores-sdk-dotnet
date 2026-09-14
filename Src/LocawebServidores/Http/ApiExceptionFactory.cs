using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using LocawebServidores.Exceptions;
using LocawebServidores.JsonApi;

namespace LocawebServidores.Http
{
    /// <summary>
    /// Maps non-success HTTP responses to the SDK exception hierarchy.
    /// </summary>
    internal static class ApiExceptionFactory
    {
        private const int MaxContentSnippetLength = 300;
        private const int UnprocessableEntity = 422;
        private const int TooManyRequests = 429;

        public static LocawebServidoresApiException Create(
            HttpStatusCode statusCode,
            string? reasonPhrase,
            string? content,
            IReadOnlyDictionary<string, IEnumerable<string>> headers,
            JsonSerializerOptions serializerOptions
        )
        {
            var errors = ParseErrors(content, serializerOptions);
            var message = BuildMessage(statusCode, reasonPhrase, errors, content);
            var code = (int)statusCode;

            switch (code)
            {
                case (int)HttpStatusCode.Unauthorized:
                case (int)HttpStatusCode.Forbidden:
                    return new LocawebServidoresAuthenticationException(
                        statusCode,
                        message,
                        content,
                        errors,
                        headers
                    );
                case (int)HttpStatusCode.NotFound:
                    return new LocawebServidoresNotFoundException(
                        statusCode,
                        message,
                        content,
                        errors,
                        headers
                    );
                case (int)HttpStatusCode.BadRequest:
                case UnprocessableEntity:
                    return new LocawebServidoresValidationException(
                        statusCode,
                        message,
                        content,
                        errors,
                        headers
                    );
                case TooManyRequests:
                    return new LocawebServidoresRateLimitException(
                        statusCode,
                        message,
                        content,
                        errors,
                        headers,
                        ParseRetryAfter(headers)
                    );
                default:
                    if (code >= 500)
                    {
                        return new LocawebServidoresServerErrorException(
                            statusCode,
                            message,
                            content,
                            errors,
                            headers
                        );
                    }

                    return new LocawebServidoresApiException(
                        statusCode,
                        message,
                        content,
                        errors,
                        headers
                    );
            }
        }

        internal static IReadOnlyList<JsonApiError> ParseErrors(
            string? content,
            JsonSerializerOptions serializerOptions
        )
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Array.Empty<JsonApiError>();
            }

            try
            {
                using (var document = JsonDocument.Parse(content!))
                {
                    var root = document.RootElement;
                    if (root.ValueKind != JsonValueKind.Object)
                    {
                        return Array.Empty<JsonApiError>();
                    }

                    if (
                        root.TryGetProperty("errors", out var errorsElement)
                        && errorsElement.ValueKind == JsonValueKind.Array
                    )
                    {
                        var parsed = JsonSerializer.Deserialize<List<JsonApiError>>(
                            errorsElement.GetRawText(),
                            serializerOptions
                        );
                        if (parsed != null && parsed.Count > 0)
                        {
                            return parsed;
                        }
                    }

                    // Tolerate non JSON:API error bodies such as {"error": "..."}.
                    foreach (var name in new[] { "error", "message", "detail", "title" })
                    {
                        if (
                            root.TryGetProperty(name, out var value)
                            && value.ValueKind == JsonValueKind.String
                        )
                        {
                            return new[] { new JsonApiError { Detail = value.GetString() } };
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON; fall through.
            }

            return Array.Empty<JsonApiError>();
        }

        internal static TimeSpan? ParseRetryAfter(
            IReadOnlyDictionary<string, IEnumerable<string>> headers
        )
        {
            if (headers is null || !headers.TryGetValue("Retry-After", out var values))
            {
                return null;
            }

            var value = values?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (
                int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var seconds
                )
            )
            {
                return TimeSpan.FromSeconds(Math.Max(0, seconds));
            }

            if (
                DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var date
                )
            )
            {
                var delay = date - DateTimeOffset.UtcNow;
                return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
            }

            return null;
        }

        private static string BuildMessage(
            HttpStatusCode statusCode,
            string? reasonPhrase,
            IReadOnlyList<JsonApiError> errors,
            string? content
        )
        {
            var reason = string.IsNullOrWhiteSpace(reasonPhrase)
                ? statusCode.ToString()
                : reasonPhrase;
            var message =
                "The Locaweb Servidores API returned HTTP " + (int)statusCode + " (" + reason + ")";

            if (errors.Count > 0)
            {
                return message + ": " + string.Join("; ", errors.Select(e => e.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var snippet = content!.Trim();
                if (snippet.Length > MaxContentSnippetLength)
                {
                    snippet = snippet.Substring(0, MaxContentSnippetLength) + "...";
                }

                return message + ": " + snippet;
            }

            return message + ".";
        }
    }
}
