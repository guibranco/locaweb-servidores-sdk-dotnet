using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LocawebServidores.Models.Requests;
using LocawebServidores.UnitTests.Helpers;
using RichardSzalay.MockHttp;
using Xunit;

namespace LocawebServidores.UnitTests.Resources
{
    public class ScalabilityResourceTests
    {
        private const string ScalabilityUrl = "cloud/servers/srv-001/scalability";

        [Fact]
        public async Task ScaleWindows_ListCreateUpdateDeleteStop()
        {
            var start = new DateTimeOffset(2026, 1, 10, 3, 0, 0, TimeSpan.Zero);
            var end = start.AddHours(6);

            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(ScalabilityUrl + "/scale_windows"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "scale_windows",
                        Fixtures.ResourceItem(
                            "scale_windows",
                            "sw-1",
                            "{\"plan\":\"cloud-8gb\",\"starts_at\":\"2026-01-10T03:00:00Z\",\"ends_at\":\"2026-01-10T09:00:00Z\",\"status\":\"scheduled\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Post, TestClient.Url(ScalabilityUrl + "/scale_windows"))
                .WithContent(
                    "{\"data\":{\"type\":\"scale_windows\",\"attributes\":{\"plan\":\"cloud-8gb\",\"starts_at\":\"2026-01-10T03:00:00+00:00\",\"ends_at\":\"2026-01-10T09:00:00+00:00\"}}}"
                )
                .Respond(
                    HttpStatusCode.Created,
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "scale_windows",
                        "sw-2",
                        "{\"plan\":\"cloud-8gb\",\"status\":\"scheduled\"}"
                    )
                );
            mock.Expect(HttpMethod.Put, TestClient.Url(ScalabilityUrl + "/scale_windows/sw-2"))
                .WithPartialContent("\"type\":\"scale_windows\",\"id\":\"sw-2\"")
                .WithPartialContent("\"plan\":\"cloud-16gb\"")
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("scale_windows", "sw-2", "{\"plan\":\"cloud-16gb\"}")
                );
            mock.Expect(
                    HttpMethod.Post,
                    TestClient.Url(ScalabilityUrl + "/scale_windows/sw-2/stop")
                )
                .With(request => request.Content == null)
                .Respond(HttpStatusCode.NoContent);
            mock.Expect(HttpMethod.Delete, TestClient.Url(ScalabilityUrl + "/scale_windows/sw-2"))
                .Respond(HttpStatusCode.NoContent);

            using var client = TestClient.Create(mock);
            var windows = await client.Scalability.ListScaleWindowsAsync("srv-001");
            var created = await client.Scalability.CreateScaleWindowAsync(
                "srv-001",
                new ScaleWindowRequest("cloud-8gb", start, end)
            );
            var updated = await client.Scalability.UpdateScaleWindowAsync(
                "srv-001",
                "sw-2",
                new ScaleWindowRequest("cloud-16gb", start, end)
            );
            await client.Scalability.StopScaleWindowAsync("srv-001", "sw-2");
            await client.Scalability.DeleteScaleWindowAsync("srv-001", "sw-2");

            Assert.Equal(start, windows.Items[0].Attributes?.StartsAt);
            Assert.Equal(end, windows.Items[0].Attributes?.EndsAt);
            Assert.Equal("scheduled", windows.Items[0].Attributes?.Status);
            Assert.Equal("sw-2", created.Id);
            Assert.Equal("cloud-16gb", updated.Attributes?.Plan);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void ScaleWindowRequest_ValidatesRange()
        {
            var now = DateTimeOffset.UtcNow;
            Assert.Throws<ArgumentException>(() => new ScaleWindowRequest("plan", now, now));
            Assert.Throws<ArgumentException>(() =>
                new ScaleWindowRequest(" ", now, now.AddHours(1))
            );
        }

        [Fact]
        public async Task EventTriggers_ListAndUpdate()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(ScalabilityUrl + "/event_triggers"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "event_triggers",
                        Fixtures.ResourceItem(
                            "event_triggers",
                            "et-cpu",
                            "{\"type\":\"cpu\",\"threshold\":80,\"enabled\":true}"
                        ),
                        Fixtures.ResourceItem(
                            "event_triggers",
                            "et-mem",
                            "{\"type\":\"memory\",\"threshold\":\"90\",\"enabled\":false}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Put, TestClient.Url(ScalabilityUrl + "/event_triggers/et-cpu"))
                .WithContent(
                    "{\"data\":{\"type\":\"event_triggers\",\"id\":\"et-cpu\",\"attributes\":{\"threshold\":85,\"enabled\":true}}}"
                )
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "event_triggers",
                        "et-cpu",
                        "{\"type\":\"cpu\",\"threshold\":85,\"enabled\":true}"
                    )
                );

            using var client = TestClient.Create(mock);
            var triggers = await client.Scalability.ListEventTriggersAsync("srv-001");
            var updated = await client.Scalability.UpdateEventTriggerAsync(
                "srv-001",
                "et-cpu",
                new UpdateEventTriggerRequest { Threshold = 85, Enabled = true }
            );

            Assert.Equal(80, triggers.Items[0].Attributes?.Threshold);
            Assert.Equal(90, triggers.Items[1].Attributes?.Threshold);
            Assert.False(triggers.Items[1].Attributes?.Enabled);
            Assert.Equal(85, updated.Attributes?.Threshold);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void UpdateEventTriggerRequest_ValidatesThreshold()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new UpdateEventTriggerRequest { Threshold = 101 }
            );
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new UpdateEventTriggerRequest { Threshold = -1 }
            );
            Assert.Equal(50, new UpdateEventTriggerRequest { Threshold = 50 }.Threshold);
        }

        [Fact]
        public async Task PlansAndScaleInfo()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(ScalabilityUrl + "/plans"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "plans",
                        Fixtures.ResourceItem(
                            "plans",
                            "cloud-8gb",
                            "{\"name\":\"Cloud 8GB\",\"cpu\":4,\"memory\":8192}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url(ScalabilityUrl + "/scale_info"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "scale_info",
                        "srv-001",
                        "{\"status\":\"scaled\",\"plan\":\"cloud-8gb\",\"scale_window_id\":\"sw-1\"}"
                    )
                );

            using var client = TestClient.Create(mock);
            var plans = await client.Scalability.ListPlansAsync("srv-001");
            var info = await client.Scalability.GetScaleInfoAsync("srv-001");

            Assert.Equal("Cloud 8GB", plans.Items[0].Attributes?.Name);
            Assert.Equal(4, plans.Items[0].Attributes?.Cpu);
            Assert.Equal(8192, plans.Items[0].Attributes?.Memory);
            Assert.Equal("scaled", info.Attributes?.Status);
            Assert.Equal("sw-1", info.Attributes?.GetAttribute<string>("scale_window_id"));
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task NullRequests_Throw()
        {
            using var client = TestClient.Create(new MockHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Scalability.CreateScaleWindowAsync("srv-001", null!)
            );
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Scalability.UpdateScaleWindowAsync("srv-001", "sw-1", null!)
            );
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.Scalability.UpdateEventTriggerAsync("srv-001", "et-1", null!)
            );
        }
    }
}
