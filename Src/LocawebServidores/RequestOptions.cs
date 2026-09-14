using System;
using System.Collections.Generic;
using System.Globalization;
using LocawebServidores.Internal;

namespace LocawebServidores
{
    /// <summary>
    /// Per-request options: query parameters (JSON:API sparse fieldsets, pagination,
    /// filters, includes, sorting) and the response locale.
    /// </summary>
    public sealed class RequestOptions
    {
        private readonly List<KeyValuePair<string, string>> _queryParameters =
            new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Gets the query parameters that will be appended to the request URL, in order.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, string>> QueryParameters => _queryParameters;

        /// <summary>
        /// Gets or sets the locale for this request (for example <c>pt-BR</c>).
        /// Overrides <see cref="LocawebServidoresClientOptions.Locale"/> when set.
        /// </summary>
        public string? Locale { get; set; }

        /// <summary>
        /// Adds an arbitrary query parameter.
        /// </summary>
        public RequestOptions WithQueryParameter(string name, string value)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            _queryParameters.Add(new KeyValuePair<string, string>(name, value ?? string.Empty));
            return this;
        }

        /// <summary>
        /// Requests a JSON:API sparse fieldset, i.e. <c>fields[resourceType]=a,b,c</c>.
        /// </summary>
        /// <param name="resourceType">The JSON:API resource type, for example <c>servers</c>.</param>
        /// <param name="fields">The attribute names to return.</param>
        public RequestOptions WithFields(string resourceType, params string[] fields)
        {
            Guard.NotNullOrWhiteSpace(resourceType, nameof(resourceType));
            Guard.NotNull(fields, nameof(fields));
            if (fields.Length == 0)
            {
                throw new ArgumentException("At least one field is required.", nameof(fields));
            }

            return WithQueryParameter("fields[" + resourceType + "]", string.Join(",", fields));
        }

        /// <summary>
        /// Adds a JSON:API filter, i.e. <c>filter[name]=value</c>.
        /// </summary>
        public RequestOptions WithFilter(string name, string value)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            return WithQueryParameter("filter[" + name + "]", value);
        }

        /// <summary>
        /// Requests JSON:API pagination, i.e. <c>page[number]=n</c> and optionally <c>page[size]=s</c>.
        /// </summary>
        public RequestOptions WithPage(int number, int? size = null)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(number),
                    "The page number must be greater than or equal to 1."
                );
            }

            WithQueryParameter("page[number]", number.ToString(CultureInfo.InvariantCulture));
            if (size.HasValue)
            {
                if (size.Value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(size),
                        "The page size must be greater than or equal to 1."
                    );
                }

                WithQueryParameter("page[size]", size.Value.ToString(CultureInfo.InvariantCulture));
            }

            return this;
        }

        /// <summary>
        /// Requests JSON:API compound documents, i.e. <c>include=a,b</c>.
        /// </summary>
        public RequestOptions WithInclude(params string[] relationships)
        {
            Guard.NotNull(relationships, nameof(relationships));
            if (relationships.Length == 0)
            {
                throw new ArgumentException(
                    "At least one relationship is required.",
                    nameof(relationships)
                );
            }

            return WithQueryParameter("include", string.Join(",", relationships));
        }

        /// <summary>
        /// Requests JSON:API sorting, i.e. <c>sort=a,-b</c>.
        /// </summary>
        public RequestOptions WithSort(params string[] fields)
        {
            Guard.NotNull(fields, nameof(fields));
            if (fields.Length == 0)
            {
                throw new ArgumentException("At least one sort field is required.", nameof(fields));
            }

            return WithQueryParameter("sort", string.Join(",", fields));
        }

        /// <summary>
        /// Sets the locale for this request (for example <c>pt-BR</c>).
        /// </summary>
        public RequestOptions WithLocale(string locale)
        {
            Locale = Guard.NotNullOrWhiteSpace(locale, nameof(locale));
            return this;
        }
    }
}
