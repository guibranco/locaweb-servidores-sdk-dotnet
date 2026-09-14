using System;
using System.Net.Http;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Resources
{
    public class CloudServersResourceTests
    {
        [Fact]
        public async Task ListAsync_ParsesResourcesMetaAndLinks()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);

            using var client = TestClient.Create(mock);
            var servers = await client.CloudServers.ListAsync();

            Assert.Equal(2, servers.Count);
            Assert.True(servers.HasNextPage);
            Assert.Equal(
                "https://api-servidores.locaweb.com.br/v1/cloud/servers?page[number]=2",
                servers.Links?.Next
            );
            Assert.Equal(2, servers.Meta?.GetProperty("total").GetInt32());

            var first = servers.Items[0];
            Assert.Equal("srv-001", first.Id);
            Assert.Equal("servers", first.Type);
            Assert.Equal("web-01", first.Attributes?.Nickname);
            Assert.Equal("on", first.Attributes?.PowerState);
            Assert.Equal("cloud-4gb", first.Attributes?.Plan);
            Assert.Equal(
                new DateTimeOffset(2024, 1, 5, 10, 0, 0, TimeSpan.FromHours(-3)),
                first.Attributes?.CreatedAt
            );
            Assert.Equal("Ubuntu 22.04", first.Attributes?.GetAttribute<string>("os"));
            Assert.Equal(
                "https://api-servidores.locaweb.com.br/v1/cloud/servers/srv-001",
                first.Links?.Self
            );

            Assert.Equal("off", servers.Items[1].Attributes?.PowerState);
            Assert.Null(servers.Items[1].Attributes?.CreatedAt);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ListAsync_ForwardsRequestOptions()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .WithQueryString("page[number]", "2")
                .WithQueryString("page[size]", "50")
                .WithQueryString("sort", "-created_at")
                .WithQueryString("filter[status]", "active")
                .WithQueryString("include", "ips")
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);

            using var client = TestClient.Create(mock);
            await client.CloudServers.ListAsync(
                new RequestOptions()
                    .WithPage(2, 50)
                    .WithSort("-created_at")
                    .WithFilter("status", "active")
                    .WithInclude("ips")
            );

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetAsync_ReturnsResourceWithAdditionalAttributes()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001"))
                .Respond(Fixtures.JsonApi, Fixtures.CloudServerDetails);

            using var client = TestClient.Create(mock);
            var server = await client.CloudServers.GetAsync("srv-001");

            Assert.Equal("srv-001", server.Id);
            Assert.Equal("web-01", server.Attributes?.Nickname);
            Assert.Equal(2, server.Attributes?.GetAttribute<int>("cpu"));
            Assert.Equal(4096, server.Attributes?.GetAttribute<int>("memory"));
            Assert.True(server.Attributes?.TryGetAttribute("cpu", out _));
            Assert.False(server.Attributes?.TryGetAttribute("disk", out _));
            Assert.Null(server.Attributes?.GetAttribute<string>("disk"));
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetAsync_EscapesServerKey()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv%20one"))
                .Respond(Fixtures.JsonApi, Fixtures.CloudServerDetails);

            using var client = TestClient.Create(mock);
            await client.CloudServers.GetAsync("srv one");

            mock.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetAsync_InvalidServerKey_Throws(string? serverKey)
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());
            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                client.CloudServers.GetAsync(serverKey!)
            );
        }

        [Fact]
        public async Task GetAsync_DocumentWithoutData_Throws()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001"))
                .Respond(Fixtures.JsonApi, "{\"data\":null}");

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresException>(() =>
                client.CloudServers.GetAsync("srv-001")
            );

            Assert.Contains("without primary data", exception.Message);
        }

        [Fact]
        public async Task GetStatusAsync_RequestsSparseFieldsetAndReadsMetaActions()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001"))
                .WithQueryString("fields[servers]", "power_state,status")
                .Respond(Fixtures.JsonApi, Fixtures.CloudServerStatus);

            using var client = TestClient.Create(mock);
            var status = await client.CloudServers.GetStatusAsync("srv-001");

            Assert.Equal("srv-001", status.Id);
            Assert.Equal("active", status.Status);
            Assert.Equal("on", status.PowerState);
            Assert.Equal(new[] { "reboot" }, status.AvailableActions);
            Assert.Equal("srv-001: status=active, power_state=on", status.ToString());
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task UpdateNicknameAsync_SendsJsonApiPutDocument()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Put, TestClient.Url("cloud/servers/srv-001"))
                .WithContent(
                    "{\"data\":{\"type\":\"servers\",\"id\":\"srv-001\",\"attributes\":{\"nickname\":\"web-02\"}}}"
                )
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "servers",
                        "srv-001",
                        "{\"name\":\"srv-001\",\"nickname\":\"web-02\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var server = await client.CloudServers.UpdateNicknameAsync("srv-001", "web-02");

            Assert.Equal("web-02", server.Attributes?.Nickname);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task UpdateNicknameAsync_EmptyNickname_Throws()
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.CloudServers.UpdateNicknameAsync("srv-001", " ")
            );
        }

        [Fact]
        public async Task ListInstallationImagesAsync_ParsesCollection()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001/installation_images"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "installation_images",
                        Fixtures.ResourceItem(
                            "installation_images",
                            "img-1",
                            "{\"name\":\"Ubuntu 22.04\"}"
                        ),
                        Fixtures.ResourceItem(
                            "installation_images",
                            "img-2",
                            "{\"name\":\"Debian 12\"}"
                        )
                    )
                );

            using var client = TestClient.Create(mock);
            var images = await client.CloudServers.ListInstallationImagesAsync("srv-001");

            Assert.Equal(2, images.Count);
            Assert.Equal("Debian 12", images.Items[1].Attributes?.Name);
            Assert.False(images.HasNextPage);
            mock.VerifyNoOutstandingExpectation();
        }
    }
}
