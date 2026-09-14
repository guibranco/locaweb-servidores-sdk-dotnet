using System;
using System.Collections.Generic;
using System.Text;

namespace LocawebServidores.Internal
{
    /// <summary>
    /// Builds URL-encoded query strings.
    /// </summary>
    internal static class QueryStringBuilder
    {
        /// <summary>
        /// Appends the given parameters to a relative path, taking into account
        /// whether the path already carries a query string.
        /// </summary>
        public static string Append(
            string path,
            IEnumerable<KeyValuePair<string, string>>? parameters
        )
        {
            if (parameters is null)
            {
                return path;
            }

            var builder = new StringBuilder(path);
            var separator = path.IndexOf('?') >= 0 ? '&' : '?';

            foreach (var parameter in parameters)
            {
                builder
                    .Append(separator)
                    .Append(Uri.EscapeDataString(parameter.Key))
                    .Append('=')
                    .Append(Uri.EscapeDataString(parameter.Value ?? string.Empty));
                separator = '&';
            }

            return builder.ToString();
        }
    }
}
