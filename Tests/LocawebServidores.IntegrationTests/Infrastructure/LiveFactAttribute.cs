using System;
using Xunit;

namespace LocawebServidores.IntegrationTests.Infrastructure
{
    /// <summary>
    /// Runs the test only when the <c>LOCAWEB_API_TOKEN</c> environment variable is set;
    /// otherwise the test is reported as skipped.
    /// </summary>
    public sealed class LiveFactAttribute : FactAttribute
    {
        public const string TokenVariable = "LOCAWEB_API_TOKEN";

        public LiveFactAttribute()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(TokenVariable)))
            {
                Skip = "Set the " + TokenVariable + " environment variable to run live API tests.";
            }
        }

        public static string GetToken()
        {
            return Environment.GetEnvironmentVariable(TokenVariable) ?? string.Empty;
        }
    }
}
