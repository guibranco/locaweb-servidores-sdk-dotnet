using System;
using System.Net.Http;

namespace LocawebServidores.IntegrationTests.Infrastructure
{
    /// <summary>
    /// xUnit class fixture that starts one <see cref="LocalApiServer"/> per test class and
    /// builds SDK clients pointed at it.
    /// </summary>
    public sealed class LocalApiServerFixture : IDisposable
    {
        public const string Token = "integration-token";

        public LocalApiServerFixture()
        {
            Server = new LocalApiServer();
            Server.Start();
        }

        public LocalApiServer Server { get; }

        public LocawebServidoresClient CreateClient(
            Action<LocawebServidoresClientOptions>? configure = null
        )
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = Token,
                BaseAddress = Server.BaseAddress,
                Timeout = TimeSpan.FromSeconds(10),
            };
            configure?.Invoke(options);
            return new LocawebServidoresClient(options);
        }

        public LocawebServidoresClient CreateClient(HttpClient httpClient)
        {
            return new LocawebServidoresClient(
                httpClient,
                new LocawebServidoresClientOptions
                {
                    ApiToken = Token,
                    BaseAddress = Server.BaseAddress,
                }
            );
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
