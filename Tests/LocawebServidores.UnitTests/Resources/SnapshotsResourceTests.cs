using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LocawebServidores.Exceptions;
using LocawebServidores.Models.Requests;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Resources
{
    public class SnapshotsResourceTests
    {
        private const string SnapshotsUrl = "cloud/servers/srv-001/snapshots";

        [Fact]
        public async Task ListAndGet_ParseSnapshots()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(SnapshotsUrl))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "snapshots",
                        Fixtures.ResourceItem(
                            "snapshots",
                            "snap-1",
                            "{\"name\":\"before-upgrade\",\"status\":\"ready\",\"created_at\":\"2024-02-01T00:00:00Z\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url(SnapshotsUrl + "/snap-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("snapshots", "snap-1", "{\"name\":\"before-upgrade\"}")
                );

            using var client = TestClient.Create(mock);
            var snapshots = await client.Snapshots.ListAsync("srv-001");
            var snapshot = await client.Snapshots.GetAsync("srv-001", "snap-1");

            Assert.Equal("ready", snapshots.Items[0].Attributes?.Status);
            Assert.Equal(
                new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero),
                snapshots.Items[0].Attributes?.CreatedAt
            );
            Assert.Equal("before-upgrade", snapshot.Attributes?.Name);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task CreateAsync_WithRequest_SendsAttributes()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url(SnapshotsUrl))
                .WithContent(
                    "{\"data\":{\"type\":\"snapshots\",\"attributes\":{\"name\":\"nightly\",\"description\":\"Nightly backup\"}}}"
                )
                .Respond(
                    HttpStatusCode.Created,
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "snapshots",
                        "snap-2",
                        "{\"name\":\"nightly\",\"status\":\"creating\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var snapshot = await client.Snapshots.CreateAsync(
                "srv-001",
                new CreateSnapshotRequest("nightly", "Nightly backup")
            );

            Assert.Equal("snap-2", snapshot.Id);
            Assert.Equal("creating", snapshot.Attributes?.Status);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task CreateAsync_WithoutRequest_SendsEmptyAttributes()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url(SnapshotsUrl))
                .WithContent("{\"data\":{\"type\":\"snapshots\",\"attributes\":{}}}")
                .Respond(Fixtures.JsonApi, Fixtures.Resource("snapshots", "snap-3", "{}"));

            using var client = TestClient.Create(mock);
            await client.Snapshots.CreateAsync("srv-001");

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task DeleteAsync_SendsDelete()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Delete, TestClient.Url(SnapshotsUrl + "/snap-1"))
                .Respond(HttpStatusCode.NoContent);

            using var client = TestClient.Create(mock);
            await client.Snapshots.DeleteAsync("srv-001", "snap-1");

            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task SnapshotActions_ListGetCreate_UseNestedPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(SnapshotsUrl + "/snap-1/actions"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "actions",
                        Fixtures.ResourceItem(
                            "actions",
                            "act-7",
                            "{\"type\":\"restore\",\"status\":\"done\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Post, TestClient.Url(SnapshotsUrl + "/snap-1/actions"))
                .WithContent(
                    "{\"data\":{\"type\":\"actions\",\"attributes\":{\"type\":\"restore\"}}}"
                )
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "actions",
                        "act-8",
                        "{\"type\":\"restore\",\"status\":\"pending\"}"
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url(SnapshotsUrl + "/snap-1/actions/act-8"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "actions",
                        "act-8",
                        "{\"type\":\"restore\",\"status\":\"done\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var actions = await client.Snapshots.ListActionsAsync("srv-001", "snap-1");
            var created = await client.Snapshots.CreateActionAsync("srv-001", "snap-1", "restore");
            var fetched = await client.Snapshots.GetActionAsync("srv-001", "snap-1", "act-8");

            Assert.Equal("act-7", actions.Items[0].Id);
            Assert.Equal("pending", created.Attributes?.Status);
            Assert.Equal("done", fetched.Attributes?.Status);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task WaitForActionCompletionAsync_PollsNestedPath()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(SnapshotsUrl + "/snap-1/actions/act-8"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("actions", "act-8", "{\"status\":\"pending\"}")
                );
            mock.Expect(HttpMethod.Get, TestClient.Url(SnapshotsUrl + "/snap-1/actions/act-8"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("actions", "act-8", "{\"status\":\"done\"}")
                );

            using var client = TestClient.Create(mock);
            var action = await client.Snapshots.WaitForActionCompletionAsync(
                "srv-001",
                "snap-1",
                "act-8",
                a => a.Attributes?.Status == "done",
                TimeSpan.FromMilliseconds(5)
            );

            Assert.Equal("done", action.Attributes?.Status);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task WaitForActionCompletionAsync_Timeout_Throws()
        {
            var mock = new MockHttpMessageHandler();
            mock.When(HttpMethod.Get, TestClient.Url(SnapshotsUrl + "/snap-1/actions/act-8"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("actions", "act-8", "{\"status\":\"pending\"}")
                );

            using var client = TestClient.Create(mock);
            await Assert.ThrowsAsync<LocawebServidoresTimeoutException>(() =>
                client.Snapshots.WaitForActionCompletionAsync(
                    "srv-001",
                    "snap-1",
                    "act-8",
                    _ => false,
                    TimeSpan.FromMilliseconds(5),
                    TimeSpan.FromMilliseconds(80)
                )
            );
        }
    }
}
