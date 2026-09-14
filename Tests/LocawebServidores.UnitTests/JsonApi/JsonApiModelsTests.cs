using System.Collections.Generic;
using System.Text.Json;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Serialization;
using Xunit;

namespace LocawebServidores.UnitTests.JsonApi
{
    public class JsonApiModelsTests
    {
        private static readonly JsonSerializerOptions Options =
            LocawebServidoresJson.CreateOptions();

        [Fact]
        public void JsonApiLinks_ResolvesStringAndObjectLinks()
        {
            var links = JsonSerializer.Deserialize<JsonApiLinks>(
                "{\"self\":\"https://a/self\",\"next\":{\"href\":\"https://a/next\",\"meta\":{}},\"prev\":null,\"last\":{\"meta\":{}},\"first\":12}",
                Options
            )!;

            Assert.Equal("https://a/self", links.Self);
            Assert.Equal("https://a/next", links.Next);
            Assert.Null(links.Prev);
            Assert.Null(links.Last);
            Assert.Null(links.First);
            Assert.Null(links.Related);
            Assert.Null(links.GetHref("missing"));
            Assert.Null(links.GetHref(""));
            Assert.Equal("https://a/self", links.GetHref("SELF"));
        }

        [Fact]
        public void JsonApiError_ToString_CombinesMembers()
        {
            var full = new JsonApiError
            {
                Status = "422",
                Title = "Invalid",
                Detail = "Name is required",
                Source = new JsonApiErrorSource { Pointer = "/data/attributes/name" },
            };
            Assert.Equal(
                "[422] Invalid: Name is required (/data/attributes/name)",
                full.ToString()
            );

            Assert.Equal("Only detail", new JsonApiError { Detail = "Only detail" }.ToString());
            Assert.Equal(
                "Only title (page)",
                new JsonApiError
                {
                    Title = "Only title",
                    Source = new JsonApiErrorSource { Parameter = "page" },
                }.ToString()
            );
            Assert.Equal("code_only", new JsonApiError { Code = "code_only" }.ToString());
            Assert.Equal("Unknown error", new JsonApiError().ToString());
        }

        [Fact]
        public void JsonApiError_LenientMembers_AcceptNumbersAndBooleans()
        {
            var error = JsonSerializer.Deserialize<JsonApiError>(
                "{\"id\":7,\"status\":422,\"code\":true,\"title\":\"t\",\"meta\":{\"x\":1}}",
                Options
            )!;

            Assert.Equal("7", error.Id);
            Assert.Equal("422", error.Status);
            Assert.Equal("true", error.Code);
            Assert.Equal(1, error.Meta?.GetProperty("x").GetInt32());
        }

        [Fact]
        public void JsonApiError_LenientMembers_HandleNullDecimalAndObject()
        {
            var error = JsonSerializer.Deserialize<JsonApiError>(
                "{\"id\":null,\"status\":4.5,\"code\":{\"nested\":false}}",
                Options
            )!;

            Assert.Null(error.Id);
            Assert.Equal("4.5", error.Status);
            Assert.Equal("{\"nested\":false}", error.Code);

            var json = JsonSerializer.Serialize(new JsonApiError { Status = "500" }, Options);
            Assert.Equal("{\"status\":\"500\"}", json);
        }

        [Fact]
        public void JsonApiResource_TryGetMetaAndGetMeta()
        {
            var resource = JsonSerializer.Deserialize<JsonApiResource<CloudServer>>(
                "{\"id\":1,\"type\":\"servers\",\"attributes\":{},\"meta\":{\"actions\":[\"reboot\"],\"count\":3,\"nothing\":null},\"relationships\":{\"ips\":{\"data\":[]}}}",
                Options
            )!;

            Assert.Equal("1", resource.Id);
            Assert.True(resource.TryGetMeta("count", out var count));
            Assert.Equal(3, count.GetInt32());
            Assert.False(resource.TryGetMeta("missing", out _));
            Assert.False(resource.TryGetMeta("", out _));
            Assert.Equal(new[] { "reboot" }, resource.GetMeta<string[]>("actions"));
            Assert.Null(resource.GetMeta<string>("nothing"));
            Assert.Null(resource.GetMeta<string>("missing"));
            Assert.Equal(JsonValueKind.Object, resource.Relationships?.ValueKind);

            var withoutMeta = new JsonApiResource<CloudServer>();
            Assert.False(withoutMeta.TryGetMeta("actions", out _));
        }

        [Fact]
        public void ResourceAttributes_RoundTripsAdditionalAttributes()
        {
            var server = JsonSerializer.Deserialize<CloudServer>(
                "{\"name\":\"srv\",\"power_state\":\"on\",\"disk\":80,\"tags\":[\"a\",\"b\"],\"nested\":{\"k\":\"v\"}}",
                Options
            )!;

            Assert.Equal("srv", server.Name);
            Assert.Equal("on", server.PowerState);
            Assert.Equal(80, server.GetAttribute<int>("disk"));
            Assert.Equal(new[] { "a", "b" }, server.GetAttribute<string[]>("tags"));
            Assert.Equal("v", server.GetAttribute<Dictionary<string, string>>("nested")?["k"]);
            Assert.False(server.TryGetAttribute("", out _));

            server.SetAttribute("disk", 160);
            server.SetAttribute("note", null);
            var json = JsonSerializer.Serialize(server, Options);

            Assert.Contains("\"name\":\"srv\"", json);
            Assert.Contains("\"power_state\":\"on\"", json);
            Assert.Contains("\"disk\":160", json);
            Assert.Contains("\"note\":null", json);
            Assert.DoesNotContain("nickname", json);
        }

        [Fact]
        public void ResourceAttributes_WithoutAdditionalAttributes_HelpersAreSafe()
        {
            var server = new CloudServer();
            Assert.False(server.TryGetAttribute("x", out _));
            Assert.Null(server.GetAttribute<string>("x"));

            server.SetAttribute("x", "y");
            Assert.Equal("y", server.GetAttribute<string>("x"));
        }

        [Fact]
        public void ResourceCollection_WithoutData_IsEmpty()
        {
            var collection = JsonSerializer.Deserialize<ResourceCollection<CloudServer>>(
                "{\"meta\":{}}",
                Options
            )!;
            Assert.Empty(collection.Items);
            Assert.Equal(0, collection.Count);
            Assert.False(collection.HasNextPage);
        }

        [Fact]
        public void ResourceDocument_ExposesResourceAlias()
        {
            var document = JsonSerializer.Deserialize<ResourceDocument<CloudServer>>(
                "{\"data\":{\"id\":\"s\",\"type\":\"servers\",\"attributes\":{\"name\":\"s\"}},\"included\":[{\"id\":\"i\",\"type\":\"ips\",\"attributes\":{\"address\":\"1.1.1.1\"}}],\"jsonapi\":{\"version\":\"1.0\"}}",
                Options
            )!;

            Assert.Same(document.Data, document.Resource);
            Assert.Equal("s", document.Resource?.Attributes?.Name);
            Assert.Equal("ips", document.Included?[0].Type);
            Assert.Equal(
                "1.1.1.1",
                document.Included?[0].Attributes.GetProperty("address").GetString()
            );
            Assert.Equal("1.0", document.JsonApi?.GetProperty("version").GetString());
        }

        [Fact]
        public void JsonApiRequest_SerializesTypeIdAndAttributes()
        {
            var json = JsonSerializer.Serialize(
                new JsonApiRequest<object>("servers", new { Nickname = "x" }, "srv"),
                Options
            );
            Assert.Equal(
                "{\"data\":{\"type\":\"servers\",\"id\":\"srv\",\"attributes\":{\"nickname\":\"x\"}}}",
                json
            );

            Assert.Throws<System.ArgumentException>(() => new JsonApiRequest<object>(" ", new { }));
        }

        [Fact]
        public void ServerStatus_FromResource_ReadsStringAndObjectActions()
        {
            var resource = JsonSerializer.Deserialize<JsonApiResource<CloudServer>>(
                "{\"id\":\"s\",\"type\":\"servers\",\"attributes\":{\"status\":\"active\",\"power_state\":\"on\"},\"meta\":{\"actions\":[\"reboot\",{\"type\":\"shutdown\"},{\"name\":\"start\"},{\"action\":\"stop\"},{\"other\":1},5]}}",
                Options
            )!;

            var status = ServerStatus.FromResource(resource);

            Assert.Equal(new[] { "reboot", "shutdown", "start", "stop" }, status.AvailableActions);
            Assert.Equal("active", status.Status);
            Assert.Equal("on", status.PowerState);
        }

        [Fact]
        public void ServerStatus_FromResource_WithoutMetaOrAttributes()
        {
            var status = ServerStatus.FromResource(new JsonApiResource<CloudServer> { Id = "s" });

            Assert.Empty(status.AvailableActions);
            Assert.Null(status.Status);
            Assert.Equal("s: status=?, power_state=?", status.ToString());
            Assert.Throws<System.ArgumentNullException>(() =>
                ServerStatus.FromResource<CloudServer>(null!)
            );
        }

        [Fact]
        public void ResourceTypes_MatchApiPaths()
        {
            Assert.Equal("servers", ResourceTypes.Servers);
            Assert.Equal("scheduled_snapshots", ResourceTypes.ScheduledSnapshots);
            Assert.Equal("custom_templates", ResourceTypes.CustomTemplates);
            Assert.Equal("scale_windows", ResourceTypes.ScaleWindows);
            Assert.Equal("event_triggers", ResourceTypes.EventTriggers);
            Assert.Equal("installation_images", ResourceTypes.InstallationImages);
        }
    }
}
