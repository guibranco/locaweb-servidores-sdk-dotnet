using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocawebServidores.IntegrationTests.Infrastructure
{
    /// <summary>
    /// A minimal in-process HTTP server that plays the role of the Locaweb Servidores API,
    /// so the SDK can be exercised end-to-end through a real <c>HttpClient</c> without
    /// touching the network.
    /// </summary>
    public sealed class LocalApiServer : IDisposable
    {
        public const string JsonApiMediaType = "application/vnd.api+json";

        private readonly HttpListener _listener = new HttpListener();
        private readonly List<Route> _routes = new List<Route>();
        private readonly ConcurrentQueue<RecordedRequest> _requests =
            new ConcurrentQueue<RecordedRequest>();
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private Task? _loop;

        public LocalApiServer()
        {
            var port = FindFreePort();
            BaseAddress = new Uri("http://localhost:" + port + "/v1/");
            _listener.Prefixes.Add(BaseAddress.AbsoluteUri);
        }

        public Uri BaseAddress { get; }

        public IReadOnlyCollection<RecordedRequest> Requests => _requests.ToArray();

        public void Start()
        {
            _listener.Start();
            _loop = Task.Run(AcceptLoopAsync);
        }

        public void Reset()
        {
            lock (_routes)
            {
                _routes.Clear();
            }

            while (_requests.TryDequeue(out _)) { }
        }

        public void Map(string method, string path, Func<RecordedRequest, LocalResponse> handler)
        {
            lock (_routes)
            {
                _routes.Add(new Route(method, path, handler));
            }
        }

        public void MapJson(string method, string path, int status, string json)
        {
            Map(method, path, _ => new LocalResponse(status, json));
        }

        public void Dispose()
        {
            _shutdown.Cancel();
            try
            {
                _listener.Stop();
                _listener.Close();
            }
            catch (ObjectDisposedException)
            {
                // Already closed.
            }

            _shutdown.Dispose();
        }

        private static int FindFreePort()
        {
            var socket = new TcpListener(IPAddress.Loopback, 0);
            socket.Start();
            try
            {
                return ((IPEndPoint)socket.LocalEndpoint).Port;
            }
            finally
            {
                socket.Stop();
            }
        }

        private async Task AcceptLoopAsync()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (HttpListenerException)
                {
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }

                _ = Task.Run(() => HandleAsync(context));
            }
        }

        private async Task HandleAsync(HttpListenerContext context)
        {
            var request = context.Request;
            string body;
            using (
                var reader = new StreamReader(
                    request.InputStream,
                    request.ContentEncoding ?? Encoding.UTF8
                )
            )
            {
                body = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            var recorded = new RecordedRequest(
                request.HttpMethod,
                request.Url!.AbsolutePath,
                request.Url.Query,
                CopyHeaders(request.Headers),
                request.ContentType,
                body
            );
            _requests.Enqueue(recorded);

            Route? route;
            lock (_routes)
            {
                route = _routes.Find(r => r.Matches(recorded));
            }

            var response = route is null
                ? new LocalResponse(
                    404,
                    "{\"errors\":[{\"status\":\"404\",\"title\":\"No route\",\"detail\":\""
                        + recorded.Method
                        + " "
                        + recorded.Path
                        + "\"}]}"
                )
                : route.Handler(recorded);

            context.Response.StatusCode = response.StatusCode;
            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            if (response.Body is null)
            {
                context.Response.ContentLength64 = 0;
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(response.Body);
                context.Response.ContentType = response.ContentType;
                context.Response.ContentLength64 = bytes.Length;
                await context
                    .Response.OutputStream.WriteAsync(bytes, 0, bytes.Length)
                    .ConfigureAwait(false);
            }

            context.Response.Close();
        }

        private static IReadOnlyDictionary<string, string> CopyHeaders(
            System.Collections.Specialized.NameValueCollection headers
        )
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in headers.AllKeys)
            {
                if (key != null)
                {
                    result[key] = headers[key] ?? string.Empty;
                }
            }

            return result;
        }

        private sealed class Route
        {
            public Route(string method, string path, Func<RecordedRequest, LocalResponse> handler)
            {
                Method = method;
                Path = path;
                Handler = handler;
            }

            public string Method { get; }

            public string Path { get; }

            public Func<RecordedRequest, LocalResponse> Handler { get; }

            public bool Matches(RecordedRequest request)
            {
                return string.Equals(Method, request.Method, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(Path, request.Path, StringComparison.Ordinal);
            }
        }
    }

    public sealed class RecordedRequest
    {
        public RecordedRequest(
            string method,
            string path,
            string query,
            IReadOnlyDictionary<string, string> headers,
            string? contentType,
            string body
        )
        {
            Method = method;
            Path = path;
            Query = query;
            Headers = headers;
            ContentType = contentType;
            Body = body;
        }

        public string Method { get; }

        public string Path { get; }

        public string Query { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public string? ContentType { get; }

        public string Body { get; }

        public string? Header(string name)
        {
            return Headers.TryGetValue(name, out var value) ? value : null;
        }
    }

    public sealed class LocalResponse
    {
        public LocalResponse(
            int statusCode,
            string? body = null,
            string contentType = LocalApiServer.JsonApiMediaType
        )
        {
            StatusCode = statusCode;
            Body = body;
            ContentType = contentType;
        }

        public int StatusCode { get; }

        public string? Body { get; }

        public string ContentType { get; }

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    }
}
