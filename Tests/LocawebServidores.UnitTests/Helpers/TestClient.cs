using System;
using RichardSzalay.MockHttp;

namespace LocawebServidores.UnitTests.Helpers
{
    internal static class TestClient
    {
        public const string Token = "unit-test-token";
        public const string BaseUrl = "https://api-servidores.locaweb.com.br/v1/";

        public static LocawebServidoresClient Create(
            MockHttpMessageHandler handler,
            Action<LocawebServidoresClientOptions>? configure = null
        )
        {
            var options = new LocawebServidoresClientOptions { ApiToken = Token };
            configure?.Invoke(options);
            return new LocawebServidoresClient(handler.ToHttpClient(), options);
        }

        public static string Url(string relativePath)
        {
            return BaseUrl + relativePath;
        }
    }
}
