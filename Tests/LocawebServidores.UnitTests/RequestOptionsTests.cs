using System;
using System.Collections.Generic;
using Xunit;

namespace LocawebServidores.UnitTests
{
    public class RequestOptionsTests
    {
        [Fact]
        public void Builders_AppendParametersInOrder()
        {
            var options = new RequestOptions()
                .WithFields("servers", "name", "status")
                .WithFilter("status", "active")
                .WithPage(3, 25)
                .WithInclude("ips", "firewalls")
                .WithSort("-created_at", "name")
                .WithQueryParameter("custom", "value")
                .WithLocale("pt-BR");

            Assert.Equal(
                new[]
                {
                    new KeyValuePair<string, string>("fields[servers]", "name,status"),
                    new KeyValuePair<string, string>("filter[status]", "active"),
                    new KeyValuePair<string, string>("page[number]", "3"),
                    new KeyValuePair<string, string>("page[size]", "25"),
                    new KeyValuePair<string, string>("include", "ips,firewalls"),
                    new KeyValuePair<string, string>("sort", "-created_at,name"),
                    new KeyValuePair<string, string>("custom", "value"),
                },
                options.QueryParameters
            );
            Assert.Equal("pt-BR", options.Locale);
        }

        [Fact]
        public void WithPage_WithoutSize_OnlyAddsNumber()
        {
            var options = new RequestOptions().WithPage(1);
            Assert.Single(options.QueryParameters);
            Assert.Equal("page[number]", options.QueryParameters[0].Key);
        }

        [Fact]
        public void Builders_ValidateArguments()
        {
            Assert.Throws<ArgumentException>(() => new RequestOptions().WithFields("servers"));
            Assert.Throws<ArgumentException>(() => new RequestOptions().WithFields(" ", "a"));
            Assert.Throws<ArgumentNullException>(() =>
                new RequestOptions().WithFields("servers", null!)
            );
            Assert.Throws<ArgumentException>(() => new RequestOptions().WithInclude());
            Assert.Throws<ArgumentException>(() => new RequestOptions().WithSort());
            Assert.Throws<ArgumentException>(() => new RequestOptions().WithFilter("", "x"));
            Assert.Throws<ArgumentException>(() =>
                new RequestOptions().WithQueryParameter("", "x")
            );
            Assert.Throws<ArgumentOutOfRangeException>(() => new RequestOptions().WithPage(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RequestOptions().WithPage(1, 0));
            Assert.Throws<ArgumentException>(() => new RequestOptions().WithLocale(" "));
        }

        [Fact]
        public void WithQueryParameter_NullValue_BecomesEmpty()
        {
            var options = new RequestOptions().WithQueryParameter("flag", null!);
            Assert.Equal(string.Empty, options.QueryParameters[0].Value);
        }
    }
}
