using System.Net.Http;
using System.Threading.Tasks;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Resources
{
    public class VpsAndDedicatedServersResourceTests
    {
        [Fact]
        public async Task Vps_ListAsync_UsesVpsPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("vps/servers"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "servers",
                        Fixtures.ResourceItem(
                            "servers",
                            "vps-1",
                            "{\"name\":\"vps-1\",\"status\":\"active\",\"plan\":\"vps-2gb\"}"
                        )
                    )
                );

            using var client = TestClient.Create(mock);
            var servers = await client.VpsServers.ListAsync();

            Assert.Single(servers.Items);
            Assert.Equal("vps-2gb", servers.Items[0].Attributes?.Plan);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Vps_GetAsync_UsesVpsPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("vps/servers/vps-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("servers", "vps-1", "{\"nickname\":\"mail\"}")
                );

            using var client = TestClient.Create(mock);
            var server = await client.VpsServers.GetAsync("vps-1");

            Assert.Equal("mail", server.Attributes?.Nickname);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Vps_GetStatusAsync_RequestsSparseFieldset()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("vps/servers/vps-1"))
                .WithQueryString("fields[servers]", "power_state,status")
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "servers",
                        "vps-1",
                        "{\"status\":\"suspended\",\"power_state\":\"off\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var status = await client.VpsServers.GetStatusAsync("vps-1");

            Assert.Equal("suspended", status.Status);
            Assert.Equal("off", status.PowerState);
            Assert.Empty(status.AvailableActions);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Dedicated_ListAsync_UsesDedicatedPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("dedicated/servers"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "servers",
                        Fixtures.ResourceItem(
                            "servers",
                            "ded-1",
                            "{\"name\":\"ded-1\",\"location\":\"SP1\"}"
                        )
                    )
                );

            using var client = TestClient.Create(mock);
            var servers = await client.DedicatedServers.ListAsync();

            Assert.Equal("SP1", servers.Items[0].Attributes?.Location);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Dedicated_GetAndStatus_UseDedicatedPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("dedicated/servers/ded-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("servers", "ded-1", "{\"status\":\"active\"}")
                );
            mock.Expect(HttpMethod.Get, TestClient.Url("dedicated/servers/ded-1"))
                .WithQueryString("fields[servers]", "power_state,status")
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "servers",
                        "ded-1",
                        "{\"status\":\"active\",\"power_state\":\"on\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var server = await client.DedicatedServers.GetAsync("ded-1");
            var status = await client.DedicatedServers.GetStatusAsync("ded-1");

            Assert.Equal("active", server.Attributes?.Status);
            Assert.Equal("on", status.PowerState);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Dedicated_UpdateNicknameAsync_SendsPut()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Put, TestClient.Url("dedicated/servers/ded-1"))
                .WithContent(
                    "{\"data\":{\"type\":\"servers\",\"id\":\"ded-1\",\"attributes\":{\"nickname\":\"storage\"}}}"
                )
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("servers", "ded-1", "{\"nickname\":\"storage\"}")
                );

            using var client = TestClient.Create(mock);
            var server = await client.DedicatedServers.UpdateNicknameAsync("ded-1", "storage");

            Assert.Equal("storage", server.Attributes?.Nickname);
            mock.VerifyNoOutstandingExpectation();
        }
    }
}
