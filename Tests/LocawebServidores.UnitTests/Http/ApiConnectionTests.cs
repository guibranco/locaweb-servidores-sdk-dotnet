using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.Http;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Http
{
    public class ApiConnectionTests
    {
        [Fact]
        public void Constructor_NullHttpClient_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ApiConnection(null!, new LocawebServidoresClientOptions { ApiToken = "t" })
            );
        }

        [Fact]
        public void Constructor_NullOptions_Throws()
        {
            using var httpClient = new HttpClient();
            Assert.Throws<ArgumentNullException>(() => new ApiConnection(httpClient, null!));
        }

        [Fact]
        public void Constructor_InvalidOptions_Throws()
        {
            using var httpClient = new HttpClient();
            var exception = Assert.Throws<ArgumentException>(() =>
                new ApiConnection(httpClient, new LocawebServidoresClientOptions())
            );
            Assert.Equal("ApiToken", exception.ParamName);
        }

        [Fact]
        public async Task Get_SendsTokenHeaderAcceptAndUserAgent()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .WithHeaders("X-Auth-Token", TestClient.Token)
                .With(request =>
                    request.Headers.Accept.Any(a => a.MediaType == "application/vnd.api+json")
                    && request.Headers.Accept.Any(a => a.MediaType == "application/json")
                    && request.Headers.UserAgent.ToString().StartsWith("LocawebServidores.NET/")
                )
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);

            using var client = TestClient.Create(mock);
            var result = await client.Connection.GetAsync<ResourceCollection<CloudServer>>(
                "cloud/servers"
            );

            Assert.Equal(2, result.Count);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Get_CustomAuthenticationHeaderAndScheme()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .WithHeaders("Authorization", "Bearer " + TestClient.Token)
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);

            using var client = TestClient.Create(
                mock,
                options =>
                {
                    options.AuthenticationHeaderName = "Authorization";
                    options.AuthenticationScheme = "Bearer";
                }
            );

            await client.Connection.GetAsync<JsonElement>("cloud/servers");
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Get_AppendsLocaleFromClientOptions()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .WithQueryString("locale", "pt-BR")
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);

            using var client = TestClient.Create(mock, options => options.Locale = "pt-BR");
            await client.Connection.GetAsync<JsonElement>("cloud/servers");

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Get_RequestLocaleOverridesClientLocale()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .WithExactQueryString("locale=en")
                .Respond(Fixtures.JsonApi, Fixtures.CloudServersList);

            using var client = TestClient.Create(mock, options => options.Locale = "pt-BR");
            await client.Connection.GetAsync<JsonElement>(
                "cloud/servers",
                new RequestOptions().WithLocale("en")
            );

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void BuildUri_EncodesQueryParametersAndHandlesExistingQuery()
        {
            using var httpClient = new HttpClient();
            var connection = new ApiConnection(
                httpClient,
                new LocawebServidoresClientOptions
                {
                    ApiToken = "t",
                    BaseAddress = new Uri("https://example.test/v1"),
                }
            );

            var options = new RequestOptions()
                .WithFields("servers", "power_state", "status")
                .WithQueryParameter("q", "a b&c");

            var uri = connection.BuildUri("/cloud/servers?existing=1", options);

            Assert.Equal(
                "https://example.test/v1/cloud/servers?existing=1&fields%5Bservers%5D=power_state%2Cstatus&q=a%20b%26c",
                uri.AbsoluteUri
            );
        }

        [Fact]
        public async Task Post_SerializesSnakeCaseBodyWithJsonApiMediaTypeWithoutCharset()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url("cloud/servers/srv-001/actions"))
                .With(request =>
                    request.Content != null
                    && request.Content.Headers.ContentType != null
                    && request.Content.Headers.ContentType.MediaType == "application/vnd.api+json"
                    && request.Content.Headers.ContentType.CharSet == null
                )
                .WithContent(
                    "{\"data\":{\"type\":\"actions\",\"attributes\":{\"type\":\"reboot\"}}}"
                )
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);

            using var client = TestClient.Create(mock);
            var body = new JsonApiRequest<object>("actions", new { Type = "reboot" });
            var result = await client.Connection.PostAsync<ResourceDocument<ServerAction>>(
                "cloud/servers/srv-001/actions",
                body
            );

            Assert.Equal("act-1", result.Resource?.Id);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Post_CustomMediaType_IsUsedForContentTypeAndAccept()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url("cloud/servers/srv-001/actions"))
                .With(request =>
                    request.Content?.Headers.ContentType?.MediaType == "application/json"
                    && request.Headers.Accept.Count == 1
                    && request.Headers.Accept.Single().MediaType == "application/json"
                )
                .Respond("application/json", Fixtures.ActionPending);

            using var client = TestClient.Create(
                mock,
                options => options.MediaType = "application/json"
            );
            await client.Connection.PostAsync<JsonElement>(
                "cloud/servers/srv-001/actions",
                new { type = "reboot" }
            );

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Delete_NoContentResponse_Succeeds()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Delete, TestClient.Url("cloud/servers/srv-001/firewalls/fw-1"))
                .Respond(HttpStatusCode.NoContent);

            using var client = TestClient.Create(mock);
            await client.Connection.DeleteAsync("cloud/servers/srv-001/firewalls/fw-1");

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Send_ReturnsRawResponseWithHeaders()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(request =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(Fixtures.CloudServersList),
                    };
                    response.Headers.Add("X-Request-Id", "abc");
                    return response;
                });

            using var client = TestClient.Create(mock);
            var response = await client.Connection.SendAsync(HttpMethod.Get, "cloud/servers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.HasContent);
            Assert.Equal("abc", response.Headers["x-request-id"].Single());
            Assert.Equal(2, response.Deserialize<ResourceCollection<CloudServer>>()?.Count);
        }

        [Fact]
        public async Task Get_EmptyBody_ThrowsSdkException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers")).Respond(HttpStatusCode.OK);

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.Contains("empty body", exception.Message);
        }

        [Fact]
        public async Task Get_InvalidJson_ThrowsSdkException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond("text/html", "<html>not json</html>");

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresException>(() =>
                client.Connection.GetAsync<ResourceCollection<CloudServer>>("cloud/servers")
            );

            Assert.IsType<JsonException>(exception.InnerException);
        }

        [Fact]
        public async Task Get_JsonNull_ThrowsSdkException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(Fixtures.JsonApi, "null");

            using var client = TestClient.Create(mock);
            await Assert.ThrowsAsync<LocawebServidoresException>(() =>
                client.Connection.GetAsync<ResourceCollection<CloudServer>>("cloud/servers")
            );
        }

        [Fact]
        public async Task Get_Unauthorized_ThrowsAuthenticationExceptionWithErrors()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(HttpStatusCode.Unauthorized, Fixtures.JsonApi, Fixtures.UnauthorizedError);

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresAuthenticationException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
            Assert.Single(exception.Errors);
            Assert.Equal("Invalid token", exception.Errors[0].Detail);
            Assert.Contains("401", exception.Message);
            Assert.Contains("Invalid token", exception.Message);
            Assert.Equal(Fixtures.UnauthorizedError, exception.ResponseContent);
        }

        [Fact]
        public async Task Get_Forbidden_ThrowsAuthenticationException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(HttpStatusCode.Forbidden);

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresAuthenticationException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
            Assert.Empty(exception.Errors);
        }

        [Fact]
        public async Task Get_NotFound_ThrowsNotFoundException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers/missing"))
                .Respond(
                    HttpStatusCode.NotFound,
                    Fixtures.JsonApi,
                    "{\"errors\":[{\"status\":\"404\",\"title\":\"Not Found\"}]}"
                );

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresNotFoundException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers/missing")
            );

            Assert.Equal("Not Found", exception.Errors[0].Title);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData((HttpStatusCode)422)]
        public async Task Put_ValidationError_ThrowsValidationExceptionWithParsedErrors(
            HttpStatusCode status
        )
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Put, TestClient.Url("cloud/servers/srv-001"))
                .Respond(status, Fixtures.JsonApi, Fixtures.ValidationErrors);

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresValidationException>(() =>
                client.Connection.PutAsync<JsonElement>("cloud/servers/srv-001", new { })
            );

            Assert.Equal(status, exception.StatusCode);
            Assert.Equal(2, exception.Errors.Count);
            Assert.Equal("422", exception.Errors[0].Status);
            Assert.Equal("/data/attributes/nickname", exception.Errors[0].Source?.Pointer);
            Assert.Equal("too_long", exception.Errors[1].Code);
            Assert.Contains("Nickname can't be blank", exception.Message);
        }

        [Fact]
        public async Task Get_TooManyRequests_ThrowsRateLimitExceptionWithRetryAfter()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(request =>
                {
                    var response = new HttpResponseMessage((HttpStatusCode)429)
                    {
                        Content = new StringContent("{\"error\":\"Throttled\"}"),
                    };
                    response.Headers.Add("Retry-After", "30");
                    return response;
                });

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresRateLimitException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.Equal(TimeSpan.FromSeconds(30), exception.RetryAfter);
            Assert.Equal("Throttled", exception.Errors[0].Detail);
        }

        [Fact]
        public async Task Get_TooManyRequests_RetryAfterAsHttpDate()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(request =>
                {
                    var response = new HttpResponseMessage((HttpStatusCode)429);
                    response.Headers.Add(
                        "Retry-After",
                        DateTimeOffset.UtcNow.AddMinutes(1).ToString("R")
                    );
                    return response;
                });

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresRateLimitException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.NotNull(exception.RetryAfter);
            Assert.InRange(
                exception.RetryAfter!.Value,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(61)
            );
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task Get_ServerError_ThrowsServerErrorException(HttpStatusCode status)
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(status, "text/html", "<h1>Oops</h1>");

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresServerErrorException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.Equal(status, exception.StatusCode);
            Assert.Empty(exception.Errors);
            Assert.Contains("<h1>Oops</h1>", exception.Message);
        }

        [Fact]
        public async Task Get_OtherClientError_ThrowsGenericApiException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(HttpStatusCode.Conflict, Fixtures.JsonApi, "{\"message\":\"Conflict\"}");

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresApiException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
            Assert.Equal("Conflict", exception.Errors[0].Detail);
        }

        [Fact]
        public async Task Get_NetworkFailure_WrapsHttpRequestException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Throw(new HttpRequestException("connection refused"));

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.IsType<HttpRequestException>(exception.InnerException);
            Assert.Contains("connection refused", exception.Message);
        }

        [Fact]
        public async Task Get_Timeout_ThrowsTimeoutException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Throw(new TaskCanceledException("timed out"));

            using var client = TestClient.Create(mock);
            var exception = await Assert.ThrowsAsync<LocawebServidoresTimeoutException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers")
            );

            Assert.IsType<TaskCanceledException>(exception.InnerException);
        }

        [Fact]
        public async Task Get_CallerCancellation_PropagatesOperationCanceledException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url("cloud/servers"))
                .Respond(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            using var client = TestClient.Create(mock);
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                client.Connection.GetAsync<JsonElement>("cloud/servers", null, cts.Token)
            );
        }

        [Fact]
        public async Task Send_NullMethodOrPath_Throws()
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Connection.SendAsync(null!, "cloud/servers")
            );
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Connection.SendAsync(HttpMethod.Get, null!)
            );
            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Connection.SendAsync(HttpMethod.Get, "   ")
            );
        }
    }
}
