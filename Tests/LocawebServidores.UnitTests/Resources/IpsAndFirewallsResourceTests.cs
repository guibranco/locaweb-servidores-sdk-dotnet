using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Resources
{
    public class IpsAndFirewallsResourceTests
    {
        [Fact]
        public async Task Ips_ListAndGet_UseIpsPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001/ips"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "ips",
                        Fixtures.ResourceItem(
                            "ips",
                            "ip-1",
                            "{\"address\":\"200.1.2.3\",\"version\":\"4\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001/ips/ip-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("ips", "ip-1", "{\"address\":\"200.1.2.3\"}")
                );

            using var client = TestClient.Create(mock);
            var ips = await client.Ips.ListAsync("srv-001");
            var ip = await client.Ips.GetAsync("srv-001", "ip-1");

            Assert.Equal("200.1.2.3", ips.Items[0].Attributes?.Address);
            Assert.Equal("4", ips.Items[0].Attributes?.Version);
            Assert.Equal("ip-1", ip.Id);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Firewalls_ListAndGet_UseFirewallsPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001/firewalls"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "firewalls",
                        Fixtures.ResourceItem(
                            "firewalls",
                            "fw-1",
                            "{\"protocol\":\"tcp\",\"initial_port\":80,\"final_port\":\"443\",\"source\":\"0.0.0.0/0\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers/srv-001/firewalls/fw-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("firewalls", "fw-1", "{\"protocol\":\"icmp\"}")
                );

            using var client = TestClient.Create(mock);
            var rules = await client.Firewalls.ListAsync("srv-001");
            var rule = await client.Firewalls.GetAsync("srv-001", "fw-1");

            Assert.Equal("tcp", rules.Items[0].Attributes?.Protocol);
            Assert.Equal(80, rules.Items[0].Attributes?.InitialPort);
            Assert.Equal(443, rules.Items[0].Attributes?.FinalPort);
            Assert.Equal("0.0.0.0/0", rules.Items[0].Attributes?.Source);
            Assert.Equal(FirewallProtocols.Icmp, rule.Attributes?.Protocol);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Firewalls_CreateAsync_SendsJsonApiDocument()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url("cloud/servers/srv-001/firewalls"))
                .WithContent(
                    "{\"data\":{\"type\":\"firewalls\",\"attributes\":{\"protocol\":\"tcp\",\"source\":\"10.0.0.0/8\",\"initial_port\":22,\"final_port\":22}}}"
                )
                .Respond(
                    HttpStatusCode.Created,
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "firewalls",
                        "fw-9",
                        "{\"protocol\":\"tcp\",\"initial_port\":22,\"final_port\":22,\"source\":\"10.0.0.0/8\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var rule = await client.Firewalls.CreateAsync(
                "srv-001",
                new CreateFirewallRuleRequest(FirewallProtocols.Tcp, "10.0.0.0/8", 22)
            );

            Assert.Equal("fw-9", rule.Id);
            Assert.Equal(22, rule.Attributes?.FinalPort);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Firewalls_CreateAsync_IcmpWithoutPorts_OmitsPorts()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url("cloud/servers/srv-001/firewalls"))
                .WithContent(
                    "{\"data\":{\"type\":\"firewalls\",\"attributes\":{\"protocol\":\"icmp\",\"source\":\"0.0.0.0/0\"}}}"
                )
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("firewalls", "fw-2", "{\"protocol\":\"icmp\"}")
                );

            using var client = TestClient.Create(mock);
            await client.Firewalls.CreateAsync(
                "srv-001",
                new CreateFirewallRuleRequest(FirewallProtocols.Icmp, "0.0.0.0/0")
            );

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void Firewalls_CreateRequest_ValidatesArguments()
        {
            Assert.Throws<ArgumentException>(() => new CreateFirewallRuleRequest("", "0.0.0.0/0"));
            Assert.Throws<ArgumentNullException>(() => new CreateFirewallRuleRequest("tcp", null!));
        }

        [Fact]
        public async Task Firewalls_DeleteAsync_SendsDelete()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Delete, TestClient.Url("cloud/servers/srv-001/firewalls/fw-1"))
                .Respond(HttpStatusCode.NoContent);

            using var client = TestClient.Create(mock);
            await client.Firewalls.DeleteAsync("srv-001", "fw-1");

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Firewalls_CreateAsync_NullRequest_Throws()
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Firewalls.CreateAsync("srv-001", null!)
            );
        }
    }
}
