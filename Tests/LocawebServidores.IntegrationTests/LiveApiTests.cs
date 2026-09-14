using System;
using System.Threading.Tasks;
using LocawebServidores.IntegrationTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace LocawebServidores.IntegrationTests
{
    /// <summary>
    /// Read-only smoke tests against the real Locaweb Servidores API. They only run when
    /// <c>LOCAWEB_API_TOKEN</c> is set; optional <c>LOCAWEB_AUTH_HEADER</c> and
    /// <c>LOCAWEB_AUTH_SCHEME</c> override the authentication header.
    /// </summary>
    public class LiveApiTests
    {
        private readonly ITestOutputHelper _output;

        public LiveApiTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [LiveFact]
        public async Task ListCloudServers_Succeeds()
        {
            using var client = CreateClient();
            var servers = await client.CloudServers.ListAsync();

            _output.WriteLine("Cloud servers: " + servers.Count);
            foreach (var server in servers.Items)
            {
                _output.WriteLine(" - " + server.Id + " (" + server.Attributes?.Nickname + ")");
            }

            Assert.NotNull(servers.Data);
        }

        [LiveFact]
        public async Task ListVpsServers_Succeeds()
        {
            using var client = CreateClient();
            var servers = await client.VpsServers.ListAsync();

            _output.WriteLine("VPS servers: " + servers.Count);
            Assert.NotNull(servers.Data);
        }

        [LiveFact]
        public async Task GetStatusOfFirstCloudServer_Succeeds()
        {
            using var client = CreateClient();
            var servers = await client.CloudServers.ListAsync();
            if (servers.Count == 0)
            {
                _output.WriteLine("No cloud servers in the account; nothing to check.");
                return;
            }

            var status = await client.CloudServers.GetStatusAsync(servers.Items[0].Id!);
            _output.WriteLine(status.ToString());
            _output.WriteLine("Available actions: " + string.Join(", ", status.AvailableActions));

            Assert.Equal(servers.Items[0].Id, status.Id);
        }

        private static LocawebServidoresClient CreateClient()
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = LiveFactAttribute.GetToken(),
                Timeout = TimeSpan.FromSeconds(30),
            };

            var header = Environment.GetEnvironmentVariable("LOCAWEB_AUTH_HEADER");
            if (!string.IsNullOrWhiteSpace(header))
            {
                options.AuthenticationHeaderName = header!;
            }

            var scheme = Environment.GetEnvironmentVariable("LOCAWEB_AUTH_SCHEME");
            if (!string.IsNullOrWhiteSpace(scheme))
            {
                options.AuthenticationScheme = scheme;
            }

            return new LocawebServidoresClient(options);
        }
    }
}
