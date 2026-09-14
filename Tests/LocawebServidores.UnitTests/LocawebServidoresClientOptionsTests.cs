using System;
using Xunit;

namespace LocawebServidores.UnitTests
{
    public class LocawebServidoresClientOptionsTests
    {
        [Fact]
        public void Defaults_MatchDocumentedValues()
        {
            var options = new LocawebServidoresClientOptions();

            Assert.Equal(
                "https://api-servidores.locaweb.com.br/v1/",
                options.BaseAddress.AbsoluteUri
            );
            Assert.Equal("X-Auth-Token", options.AuthenticationHeaderName);
            Assert.Null(options.AuthenticationScheme);
            Assert.Null(options.Locale);
            Assert.Equal("application/vnd.api+json", options.MediaType);
            Assert.Equal(TimeSpan.FromSeconds(100), options.Timeout);
            Assert.StartsWith("LocawebServidores.NET/", options.UserAgent);
        }

        [Fact]
        public void Validate_ValidOptions_DoesNotThrow()
        {
            var options = new LocawebServidoresClientOptions { ApiToken = "token" };
            options.Validate();
        }

        [Fact]
        public void Validate_MissingToken_Throws()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new LocawebServidoresClientOptions().Validate()
            );
            Assert.Equal("ApiToken", exception.ParamName);
        }

        [Fact]
        public void Validate_RelativeBaseAddress_Throws()
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = "token",
                BaseAddress = new Uri("v1/", UriKind.Relative),
            };

            var exception = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Equal("BaseAddress", exception.ParamName);
        }

        [Fact]
        public void Validate_NullBaseAddress_Throws()
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = "token",
                BaseAddress = null!,
            };
            Assert.Throws<ArgumentException>(() => options.Validate());
        }

        [Fact]
        public void Validate_EmptyHeaderName_Throws()
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = "token",
                AuthenticationHeaderName = " ",
            };
            var exception = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Equal("AuthenticationHeaderName", exception.ParamName);
        }

        [Fact]
        public void Validate_EmptyMediaType_Throws()
        {
            var options = new LocawebServidoresClientOptions { ApiToken = "token", MediaType = "" };
            var exception = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Equal("MediaType", exception.ParamName);
        }

        [Fact]
        public void Validate_NonPositiveTimeout_Throws()
        {
            var options = new LocawebServidoresClientOptions
            {
                ApiToken = "token",
                Timeout = TimeSpan.Zero,
            };
            var exception = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Equal("Timeout", exception.ParamName);
        }
    }
}
