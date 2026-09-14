using System;
using System.Reflection;

namespace LocawebServidores
{
    /// <summary>
    /// Configuration for <see cref="LocawebServidoresClient"/>.
    /// </summary>
    public sealed class LocawebServidoresClientOptions
    {
        /// <summary>
        /// The default base address of the Locaweb Servidores API (version 1).
        /// </summary>
        public const string DefaultBaseAddress = "https://api-servidores.locaweb.com.br/v1/";

        /// <summary>
        /// The default HTTP header used to send the API token.
        /// </summary>
        /// <remarks>
        /// Locaweb's public APIs published on the Developer Network use the
        /// <c>X-Auth-Token</c> header. If your account requires a different scheme
        /// (for example <c>Authorization: Bearer</c>), set
        /// <see cref="AuthenticationHeaderName"/> and <see cref="AuthenticationScheme"/>.
        /// </remarks>
        public const string DefaultAuthenticationHeaderName = "X-Auth-Token";

        /// <summary>
        /// The default media type used for requests and accepted for responses
        /// (the API follows the JSON:API specification).
        /// </summary>
        public const string DefaultMediaType = "application/vnd.api+json";

        /// <summary>
        /// The default request timeout applied when the SDK creates its own <c>HttpClient</c>.
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);

        private static readonly string DefaultUserAgentValue = BuildDefaultUserAgent();

        /// <summary>
        /// Gets or sets the API token generated in the "API" area of the Locaweb
        /// Cloud/Dedicated servers panel. Required.
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base address of the API. Defaults to <see cref="DefaultBaseAddress"/>.
        /// </summary>
        public Uri BaseAddress { get; set; } = new Uri(DefaultBaseAddress);

        /// <summary>
        /// Gets or sets the name of the HTTP header that carries the API token.
        /// Defaults to <see cref="DefaultAuthenticationHeaderName"/>.
        /// </summary>
        public string AuthenticationHeaderName { get; set; } = DefaultAuthenticationHeaderName;

        /// <summary>
        /// Gets or sets an optional scheme prefixed to the token in the authentication
        /// header (for example <c>Bearer</c>). When <c>null</c> or empty the raw token is sent.
        /// </summary>
        public string? AuthenticationScheme { get; set; }

        /// <summary>
        /// Gets or sets the locale sent with every request (for example <c>pt-BR</c>).
        /// The API answers in English by default; the <c>locale</c> query parameter
        /// switches messages to Portuguese. <c>null</c> sends no locale.
        /// </summary>
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets the media type used for request bodies and the primary
        /// <c>Accept</c> value. Defaults to <see cref="DefaultMediaType"/>.
        /// </summary>
        public string MediaType { get; set; } = DefaultMediaType;

        /// <summary>
        /// Gets or sets the <c>User-Agent</c> header value.
        /// </summary>
        public string UserAgent { get; set; } = DefaultUserAgentValue;

        /// <summary>
        /// Gets or sets the request timeout. Only applied when the SDK creates its own
        /// <c>HttpClient</c>; when you supply an <c>HttpClient</c> configure its timeout yourself.
        /// </summary>
        public TimeSpan Timeout { get; set; } = DefaultTimeout;

        /// <summary>
        /// Validates the options, throwing when a required value is missing or invalid.
        /// </summary>
        /// <exception cref="ArgumentException">A value is missing or invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ApiToken))
            {
                throw new ArgumentException(
                    "An API token is required. Generate one in the API area of the Locaweb servers panel.",
                    nameof(ApiToken)
                );
            }

            if (BaseAddress is null || !BaseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException(
                    "The base address must be an absolute URI.",
                    nameof(BaseAddress)
                );
            }

            if (string.IsNullOrWhiteSpace(AuthenticationHeaderName))
            {
                throw new ArgumentException(
                    "The authentication header name cannot be empty.",
                    nameof(AuthenticationHeaderName)
                );
            }

            if (string.IsNullOrWhiteSpace(MediaType))
            {
                throw new ArgumentException("The media type cannot be empty.", nameof(MediaType));
            }

            if (Timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException("The timeout must be positive.", nameof(Timeout));
            }
        }

        private static string BuildDefaultUserAgent()
        {
            var version = typeof(LocawebServidoresClientOptions)
                .GetTypeInfo()
                .Assembly.GetName()
                .Version;
            var versionText = version is null ? "1.0.0" : version.ToString(3);
            return "LocawebServidores.NET/" + versionText;
        }
    }
}
