using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.IntegrationTests.Infrastructure;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;
using Xunit;

namespace LocawebServidores.IntegrationTests
{
    /// <summary>
    /// Drives the SDK through a real <c>HttpClient</c> against an in-process JSON:API server.
    /// </summary>
    public class EndToEndTests : IClassFixture<LocalApiServerFixture>
    {
        private readonly LocalApiServerFixture _fixture;

        public EndToEndTests(LocalApiServerFixture fixture)
        {
            _fixture = fixture;
            _fixture.Server.Reset();
        }

        [Fact]
        public async Task ListCloudServers_SendsAuthenticationAndParsesDocument()
        {
            _fixture.Server.MapJson(
                "GET",
                "/v1/cloud/servers",
                200,
                "{\"data\":[{\"id\":\"srv-1\",\"type\":\"servers\",\"attributes\":{\"name\":\"srv-1\",\"nickname\":\"web\",\"status\":\"active\",\"power_state\":\"on\"}}],\"links\":{\"self\":\"/v1/cloud/servers\"}}"
            );

            using var client = _fixture.CreateClient();
            var servers = await client.CloudServers.ListAsync();

            Assert.Single(servers.Items);
            Assert.Equal("web", servers.Items[0].Attributes?.Nickname);

            var request = Assert.Single(_fixture.Server.Requests);
            Assert.Equal(LocalApiServerFixture.Token, request.Header("X-Auth-Token"));
            Assert.Contains("application/vnd.api+json", request.Header("Accept"));
            Assert.StartsWith("LocawebServidores.NET/", request.Header("User-Agent"));
            Assert.Equal(string.Empty, request.Query);
        }

        [Fact]
        public async Task GetStatus_SendsSparseFieldsetAndLocale()
        {
            _fixture.Server.MapJson(
                "GET",
                "/v1/cloud/servers/srv-1",
                200,
                "{\"data\":{\"id\":\"srv-1\",\"type\":\"servers\",\"attributes\":{\"status\":\"active\",\"power_state\":\"off\"},\"meta\":{\"actions\":[\"reboot\",\"start\"]}}}"
            );

            using var client = _fixture.CreateClient(options => options.Locale = "pt-BR");
            var status = await client.CloudServers.GetStatusAsync("srv-1");

            Assert.Equal("off", status.PowerState);
            Assert.Equal(new[] { "reboot", "start" }, status.AvailableActions);

            var request = Assert.Single(_fixture.Server.Requests);
            Assert.Contains("fields%5Bservers%5D=power_state%2Cstatus", request.Query);
            Assert.Contains("locale=pt-BR", request.Query);
        }

        [Fact]
        public async Task RebootAndWait_PostsJsonApiDocumentAndPollsUntilCompleted()
        {
            var polls = 0;
            _fixture.Server.Map(
                "POST",
                "/v1/cloud/servers/srv-1/actions",
                _ => new LocalResponse(
                    201,
                    "{\"data\":{\"id\":\"act-1\",\"type\":\"actions\",\"attributes\":{\"type\":\"reboot\",\"status\":\"pending\"}}}"
                )
            );
            _fixture.Server.Map(
                "GET",
                "/v1/cloud/servers/srv-1/actions/act-1",
                _ =>
                {
                    var status = Interlocked.Increment(ref polls) >= 3 ? "completed" : "running";
                    return new LocalResponse(
                        200,
                        "{\"data\":{\"id\":\"act-1\",\"type\":\"actions\",\"attributes\":{\"type\":\"reboot\",\"status\":\""
                            + status
                            + "\"}}}"
                    );
                }
            );

            using var client = _fixture.CreateClient();
            var action = await client.Actions.RebootAsync("srv-1");
            var completed = await client.Actions.WaitForCompletionAsync(
                "srv-1",
                action.Id!,
                a => a.Attributes?.Status == "completed",
                pollInterval: TimeSpan.FromMilliseconds(10),
                timeout: TimeSpan.FromSeconds(10)
            );

            Assert.Equal("completed", completed.Attributes?.Status);
            Assert.Equal(3, polls);

            var post = _fixture.Server.Requests.First(r => r.Method == "POST");
            Assert.Equal("application/vnd.api+json", post.ContentType);
            Assert.Equal(
                "{\"data\":{\"type\":\"actions\",\"attributes\":{\"type\":\"reboot\"}}}",
                post.Body
            );
        }

        [Fact]
        public async Task CreateFirewallRule_ThenDelete_RoundTrips()
        {
            _fixture.Server.Map(
                "POST",
                "/v1/cloud/servers/srv-1/firewalls",
                request => new LocalResponse(
                    201,
                    "{\"data\":{\"id\":\"fw-1\",\"type\":\"firewalls\",\"attributes\":"
                        + ExtractAttributes(request.Body)
                        + "}}"
                )
            );
            _fixture.Server.Map(
                "DELETE",
                "/v1/cloud/servers/srv-1/firewalls/fw-1",
                _ => new LocalResponse(204)
            );

            using var client = _fixture.CreateClient();
            var rule = await client.Firewalls.CreateAsync(
                "srv-1",
                new CreateFirewallRuleRequest(FirewallProtocols.Tcp, "0.0.0.0/0", 443)
            );
            await client.Firewalls.DeleteAsync("srv-1", rule.Id!);

            Assert.Equal("tcp", rule.Attributes?.Protocol);
            Assert.Equal(443, rule.Attributes?.InitialPort);
            Assert.Equal(443, rule.Attributes?.FinalPort);
            Assert.Equal("0.0.0.0/0", rule.Attributes?.Source);
            Assert.Equal(
                new[] { "POST", "DELETE" },
                _fixture.Server.Requests.Select(r => r.Method)
            );
        }

        [Fact]
        public async Task NotFound_IsMappedToNotFoundException()
        {
            using var client = _fixture.CreateClient();

            var exception = await Assert.ThrowsAsync<LocawebServidoresNotFoundException>(() =>
                client.CloudServers.GetAsync("does-not-exist")
            );

            Assert.Equal("No route", exception.Errors.Single().Title);
            Assert.Contains("GET /v1/cloud/servers/does-not-exist", exception.Message);
        }

        [Fact]
        public async Task ValidationError_IsMappedWithParsedErrors()
        {
            _fixture.Server.MapJson(
                "PUT",
                "/v1/cloud/servers/srv-1",
                422,
                "{\"errors\":[{\"status\":\"422\",\"title\":\"Invalid nickname\",\"detail\":\"is too long\",\"source\":{\"pointer\":\"/data/attributes/nickname\"}}]}"
            );

            using var client = _fixture.CreateClient();
            var exception = await Assert.ThrowsAsync<LocawebServidoresValidationException>(() =>
                client.CloudServers.UpdateNicknameAsync("srv-1", new string('x', 300))
            );

            Assert.Equal("/data/attributes/nickname", exception.Errors.Single().Source?.Pointer);
        }

        [Fact]
        public async Task Unauthorized_IsMappedToAuthenticationException()
        {
            _fixture.Server.Map(
                "GET",
                "/v1/vps/servers",
                request =>
                    request.Header("Authorization") == "Bearer good"
                        ? new LocalResponse(200, "{\"data\":[]}")
                        : new LocalResponse(
                            401,
                            "{\"errors\":[{\"status\":\"401\",\"detail\":\"Invalid token\"}]}"
                        )
            );

            using var badClient = _fixture.CreateClient(options =>
            {
                options.AuthenticationHeaderName = "Authorization";
                options.AuthenticationScheme = "Bearer";
                options.ApiToken = "bad";
            });
            using var goodClient = _fixture.CreateClient(options =>
            {
                options.AuthenticationHeaderName = "Authorization";
                options.AuthenticationScheme = "Bearer";
                options.ApiToken = "good";
            });

            await Assert.ThrowsAsync<LocawebServidoresAuthenticationException>(() =>
                badClient.VpsServers.ListAsync()
            );
            var servers = await goodClient.VpsServers.ListAsync();

            Assert.Empty(servers.Items);
        }

        [Fact]
        public async Task RateLimit_ExposesRetryAfter()
        {
            _fixture.Server.Map(
                "GET",
                "/v1/dedicated/servers",
                _ =>
                {
                    var response = new LocalResponse(
                        429,
                        "{\"errors\":[{\"status\":\"429\",\"title\":\"Too Many Requests\"}]}"
                    );
                    response.Headers["Retry-After"] = "12";
                    return response;
                }
            );

            using var client = _fixture.CreateClient();
            var exception = await Assert.ThrowsAsync<LocawebServidoresRateLimitException>(() =>
                client.DedicatedServers.ListAsync()
            );

            Assert.Equal(TimeSpan.FromSeconds(12), exception.RetryAfter);
        }

        [Fact]
        public async Task ExternalHttpClient_IsUsedWithoutMutation()
        {
            _fixture.Server.MapJson(
                "GET",
                "/v1/cloud/servers/srv-1/ips",
                200,
                "{\"data\":[{\"id\":\"ip-1\",\"type\":\"ips\",\"attributes\":{\"address\":\"10.0.0.1\"}}]}"
            );

            using var httpClient = new HttpClient();
            using var client = _fixture.CreateClient(httpClient);
            var ips = await client.Ips.ListAsync("srv-1");

            Assert.Equal("10.0.0.1", ips.Items[0].Attributes?.Address);
            Assert.Null(httpClient.BaseAddress);
            Assert.Empty(httpClient.DefaultRequestHeaders);
        }

        [Fact]
        public async Task RawConnection_CanCallUnmodeledEndpoint()
        {
            _fixture.Server.MapJson(
                "GET",
                "/v1/cloud/servers/srv-1/network_interfaces",
                200,
                "{\"data\":[{\"id\":\"eth0\",\"type\":\"network_interfaces\",\"attributes\":{\"mac\":\"00:11\"}}]}"
            );

            using var client = _fixture.CreateClient();
            var response = await client.Connection.SendAsync(
                HttpMethod.Get,
                "cloud/servers/srv-1/network_interfaces"
            );
            var document =
                response.Deserialize<JsonApi.JsonApiDocument<System.Text.Json.JsonElement>>();

            Assert.Equal("eth0", document?.Data[0].GetProperty("id").GetString());
        }

        private static string ExtractAttributes(string body)
        {
            using var document = System.Text.Json.JsonDocument.Parse(body);
            return document.RootElement.GetProperty("data").GetProperty("attributes").GetRawText();
        }
    }
}
