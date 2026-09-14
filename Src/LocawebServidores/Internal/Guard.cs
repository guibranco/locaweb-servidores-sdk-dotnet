using System;

namespace LocawebServidores.Internal
{
    /// <summary>
    /// Argument validation helpers shared across the SDK.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Ensures the value is not <c>null</c>.
        /// </summary>
        public static T NotNull<T>(T? value, string paramName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }

        /// <summary>
        /// Ensures the value is neither <c>null</c>, empty nor white space.
        /// </summary>
        public static string NotNullOrWhiteSpace(string? value, string paramName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "The value cannot be empty or consist only of white space.",
                    paramName
                );
            }

            return value;
        }
    }
}
