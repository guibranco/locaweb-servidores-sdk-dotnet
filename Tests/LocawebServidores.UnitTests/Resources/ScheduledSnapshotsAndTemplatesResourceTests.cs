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
    public class ScheduledSnapshotsAndTemplatesResourceTests
    {
        private const string SchedulesUrl = "cloud/servers/srv-001/scheduled_snapshots";
        private const string TemplatesUrl = "cloud/servers/srv-001/custom_templates";

        [Fact]
        public async Task ScheduledSnapshots_ListAndGet()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(SchedulesUrl))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "scheduled_snapshots",
                        Fixtures.ResourceItem(
                            "scheduled_snapshots",
                            "sch-1",
                            "{\"frequency\":\"weekly\",\"time\":\"03:00\",\"day_of_week\":\"sunday\",\"retention\":\"4\",\"status\":\"active\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url(SchedulesUrl + "/sch-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource("scheduled_snapshots", "sch-1", "{\"frequency\":\"daily\"}")
                );

            using var client = TestClient.Create(mock);
            var schedules = await client.ScheduledSnapshots.ListAsync("srv-001");
            var schedule = await client.ScheduledSnapshots.GetAsync("srv-001", "sch-1");

            var attributes = schedules.Items[0].Attributes;
            Assert.Equal("weekly", attributes?.Frequency);
            Assert.Equal("03:00", attributes?.Time);
            Assert.Equal("sunday", attributes?.DayOfWeek);
            Assert.Equal(4, attributes?.Retention);
            Assert.Equal("daily", schedule.Attributes?.Frequency);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ScheduledSnapshots_CreateAndDelete()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Post, TestClient.Url(SchedulesUrl))
                .WithContent(
                    "{\"data\":{\"type\":\"scheduled_snapshots\",\"attributes\":{\"frequency\":\"weekly\",\"time\":\"02:30\",\"day_of_week\":\"monday\",\"retention\":3}}}"
                )
                .Respond(
                    HttpStatusCode.Created,
                    Fixtures.JsonApi,
                    Fixtures.Resource("scheduled_snapshots", "sch-2", "{\"frequency\":\"weekly\"}")
                );
            mock.Expect(HttpMethod.Delete, TestClient.Url(SchedulesUrl + "/sch-2"))
                .Respond(HttpStatusCode.NoContent);

            using var client = TestClient.Create(mock);
            var created = await client.ScheduledSnapshots.CreateAsync(
                "srv-001",
                new CreateScheduledSnapshotRequest("weekly", "02:30", "monday", 3)
            );
            await client.ScheduledSnapshots.DeleteAsync("srv-001", created.Id!);

            Assert.Equal("sch-2", created.Id);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void ScheduledSnapshots_CreateRequest_ValidatesArguments()
        {
            Assert.Throws<ArgumentException>(() => new CreateScheduledSnapshotRequest("", "03:00"));
            Assert.Throws<ArgumentNullException>(() =>
                new CreateScheduledSnapshotRequest("daily", null!)
            );
        }

        [Fact]
        public async Task CustomTemplates_ListGetCreateDelete()
        {
            var mock = new MockHttpMessageHandler();
            mock.Expect(HttpMethod.Get, TestClient.Url(TemplatesUrl))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Collection(
                        "custom_templates",
                        Fixtures.ResourceItem(
                            "custom_templates",
                            "tpl-1",
                            "{\"name\":\"golden\",\"status\":\"ready\"}"
                        )
                    )
                );
            mock.Expect(HttpMethod.Get, TestClient.Url(TemplatesUrl + "/tpl-1"))
                .Respond(
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "custom_templates",
                        "tpl-1",
                        "{\"name\":\"golden\",\"description\":\"Base image\"}"
                    )
                );
            mock.Expect(HttpMethod.Post, TestClient.Url(TemplatesUrl))
                .WithContent(
                    "{\"data\":{\"type\":\"custom_templates\",\"attributes\":{\"name\":\"golden-v2\",\"description\":\"Updated\"}}}"
                )
                .Respond(
                    HttpStatusCode.Created,
                    Fixtures.JsonApi,
                    Fixtures.Resource(
                        "custom_templates",
                        "tpl-2",
                        "{\"name\":\"golden-v2\",\"status\":\"creating\"}"
                    )
                );
            mock.Expect(HttpMethod.Delete, TestClient.Url(TemplatesUrl + "/tpl-2"))
                .Respond(HttpStatusCode.NoContent);

            using var client = TestClient.Create(mock);
            var templates = await client.CustomTemplates.ListAsync("srv-001");
            var template = await client.CustomTemplates.GetAsync("srv-001", "tpl-1");
            var created = await client.CustomTemplates.CreateAsync(
                "srv-001",
                new CreateCustomTemplateRequest("golden-v2", "Updated")
            );
            await client.CustomTemplates.DeleteAsync("srv-001", "tpl-2");

            Assert.Equal("golden", templates.Items[0].Attributes?.Name);
            Assert.Equal("Base image", template.Attributes?.Description);
            Assert.Equal("creating", created.Attributes?.Status);
            mock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void CustomTemplates_CreateRequest_RequiresName()
        {
            Assert.Throws<ArgumentException>(() => new CreateCustomTemplateRequest(" "));
        }
    }
}
