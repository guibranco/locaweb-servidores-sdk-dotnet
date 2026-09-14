using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Resources
{
    public class ServerActionsResourceTests
    {
        private const string ActionsUrl = "cloud/servers/srv-001/actions";

        [Fact]
        public async Task ListAsync_ParsesActions()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(ActionsUrl))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "actions",
                        Fixtures.ResourceItem(
                            "actions",
                            "act-1",
                            "{\"type\":\"reboot\",\"status\":\"completed\"}"
                        ),
                        Fixtures.ResourceItem(
                            "actions",
                            "act-2",
                            "{\"type\":\"shutdown\",\"status\":\"pending\"}"
                        )
                    )
                );

            using var client = TestClient.Create(mock);
            var actions = await client.Actions.ListAsync("srv-001");

            Assert.Equal(2, actions.Count);
            Assert.Equal("shutdown", actions.Items[1].Attributes?.Type);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetAsync_ParsesAction()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(ActionsUrl + "/act-1"))
                .Respond(Fixtures.JsonApi, Fixtures.ActionCompleted);

            using var client = TestClient.Create(mock);
            var action = await client.Actions.GetAsync("srv-001", "act-1");

            Assert.Equal("completed", action.Attributes?.Status);
            Assert.Equal(
                new DateTimeOffset(2024, 1, 5, 10, 5, 0, TimeSpan.Zero),
                action.Attributes?.UpdatedAt
            );
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RebootAsync_PostsRebootAction()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url(ActionsUrl))
                .WithContent(
                    "{\"data\":{\"type\":\"actions\",\"attributes\":{\"type\":\"reboot\"}}}"
                )
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);

            using var client = TestClient.Create(mock);
            var action = await client.Actions.RebootAsync("srv-001");

            Assert.Equal("act-1", action.Id);
            Assert.Equal("pending", action.Attributes?.Status);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task CreateAsync_WithExtraAttributes_SendsThem()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url(ActionsUrl))
                .WithContent(
                    "{\"data\":{\"type\":\"actions\",\"attributes\":{\"type\":\"reinstall\",\"image_id\":\"img-2\",\"force\":true}}}"
                )
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);

            var request = new CreateActionRequest("reinstall");
            request.SetAttribute("image_id", "img-2");
            request.SetAttribute("force", true);

            using var client = TestClient.Create(mock);
            await client.Actions.CreateAsync("srv-001", request);

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task CreateAsync_NullRequest_Throws()
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Actions.CreateAsync("srv-001", (CreateActionRequest)null!)
            );
        }

        [Fact]
        public async Task WaitForCompletionAsync_PollsUntilPredicateIsSatisfied()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(ActionsUrl + "/act-1"))
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);
            mock.Expect(HttpMethod.Get, TestClient.Url(ActionsUrl + "/act-1"))
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);
            mock.Expect(HttpMethod.Get, TestClient.Url(ActionsUrl + "/act-1"))
                .Respond(Fixtures.JsonApi, Fixtures.ActionCompleted);

            using var client = TestClient.Create(mock);
            var action = await client.Actions.WaitForCompletionAsync(
                "srv-001",
                "act-1",
                a => a.Attributes?.Status == "completed",
                pollInterval: TimeSpan.FromMilliseconds(5)
            );

            Assert.Equal("completed", action.Attributes?.Status);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task WaitForCompletionAsync_Timeout_ThrowsTimeoutException()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url(ActionsUrl + "/act-1"))
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);

            using var client = TestClient.Create(mock);
            await Assert.ThrowsAsync<LocawebServidoresTimeoutException>(() =>
                client.Actions.WaitForCompletionAsync(
                    "srv-001",
                    "act-1",
                    _ => false,
                    pollInterval: TimeSpan.FromMilliseconds(5),
                    timeout: TimeSpan.FromMilliseconds(100)
                )
            );
        }

        [Fact]
        public async Task WaitForCompletionAsync_CallerCancellation_PropagatesCancellation()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url(ActionsUrl + "/act-1"))
                .Respond(Fixtures.JsonApi, Fixtures.ActionPending);

            using var client = TestClient.Create(mock);
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                client.Actions.WaitForCompletionAsync(
                    "srv-001",
                    "act-1",
                    _ => false,
                    pollInterval: TimeSpan.FromSeconds(5),
                    cancellationToken: cts.Token
                )
            );
        }

        [Fact]
        public async Task WaitForCompletionAsync_InvalidArguments_Throw()
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Actions.WaitForCompletionAsync("srv-001", "act-1", null!)
            );
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                client.Actions.WaitForCompletionAsync("srv-001", "act-1", _ => true, TimeSpan.Zero)
            );
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                client.Actions.WaitForCompletionAsync(
                    "srv-001",
                    "act-1",
                    _ => true,
                    null,
                    TimeSpan.FromSeconds(-1)
                )
            );
            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Actions.WaitForCompletionAsync("srv-001", "", _ => true)
            );
        }

        [Fact]
        public void ServerActionTypes_Reboot_IsLowercase()
        {
            Assert.Equal("reboot", ServerActionTypes.Reboot);
        }
    }
}
