using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.UnitTests.Helpers;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests
{
    public class LocawebServidoresClientTests
    {
        [Fact]
        public void Constructor_WithToken_ExposesAllResources()
        {
            using var client = new LocawebServidoresClient("token");

            Assert.NotNull(client.Connection);
            Assert.NotNull(client.CloudServers);
            Assert.NotNull(client.VpsServers);
            Assert.NotNull(client.DedicatedServers);
            Assert.NotNull(client.Actions);
            Assert.NotNull(client.Ips);
            Assert.NotNull(client.Firewalls);
            Assert.NotNull(client.Snapshots);
            Assert.NotNull(client.ScheduledSnapshots);
            Assert.NotNull(client.CustomTemplates);
            Assert.NotNull(client.Scalability);
            Assert.Equal("token", client.Connection.Options.ApiToken);
            Assert.Equal(
                new Uri(LocawebServidoresClientOptions.DefaultBaseAddress),
                client.Connection.Options.BaseAddress
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithMissingToken_Throws(string? token)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new LocawebServidoresClient(token!)
            );
            Assert.Equal("ApiToken", exception.ParamName);
        }

        [Fact]
        public void Constructor_NullOptions_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LocawebServidoresClient((LocawebServidoresClientOptions)null!)
            );
        }

        [Fact]
        public void Constructor_NullConnection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LocawebServidoresClient((IApiConnection)null!)
            );
        }

        [Fact]
        public void Constructor_NullHttpClient_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LocawebServidoresClient(
                    (HttpClient)null!,
                    new LocawebServidoresClientOptions { ApiToken = "t" }
                )
            );
        }

        [Fact]
        public void Constructor_WithOptions_AppliesTimeoutToOwnedHttpClient()
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = "t",
                Timeout = TimeSpan.FromSeconds(7),
            };

            using var client = new LocawebServidoresClient(options);

            Assert.Same(options, client.Connection.Options);
        }

        [Fact]
        public async Task Dispose_DoesNotDisposeExternalHttpClient()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);
            using var httpClient = mock.ToHttpClient();

            var client = new LocawebServidoresClient(
                httpClient,
                new LocawebServidoresClientOptions { ApiToken = "t" }
            );
            client.Dispose();
            client.Dispose();

            var servers = await client.CloudServers.ListAsync();
            Assert.Equal(2, servers.Count);
        }

        [Fact]
        public async Task Dispose_DisposesOwnedHttpClient()
        {
            var client = new LocawebServidoresClient("t");
            client.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                client.CloudServers.ListAsync()
            );
        }

        [Fact]
        public async Task Constructor_WithCustomConnection_RoutesCallsThroughIt()
        {
            var connection = new Mock<IApiConnection>(MockBehavior.Strict);
            connection
                .Setup(c =>
                    c.GetAsync<ResourceCollection<CloudServer>>(
                        "cloud/servers",
                        null,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(
                    JsonSerializer.Deserialize<ResourceCollection<CloudServer>>(
                        Fixtures.CloudServersList,
                        Serialization.LocawebServidoresJson.CreateOptions()
                    )!
                );

            using var client = new LocawebServidoresClient(connection.Object);
            var servers = await client.CloudServers.ListAsync();

            Assert.Equal(2, servers.Count);
            Assert.Same(connection.Object, client.Connection);
            connection.VerifyAll();
        }
    }
}
