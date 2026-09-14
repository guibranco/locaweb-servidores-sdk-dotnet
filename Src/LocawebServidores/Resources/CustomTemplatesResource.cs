using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.Http;
using LocawebServidores.Internal;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Default implementation of <see cref="ICustomTemplatesResource"/>.
    /// </summary>
    public sealed class CustomTemplatesResource : ApiResource, ICustomTemplatesResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTemplatesResource"/> class.
        /// </summary>
        public CustomTemplatesResource(IApiConnection connection)
            : base(connection) { }

        /// <inheritdoc />
        public Task<ResourceCollection<CustomTemplate>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetCollectionAsync<CustomTemplate>(
                TemplatesPath(serverKey),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<CustomTemplate>> GetAsync(
            string serverKey,
            string templateId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return GetResourceAsync<CustomTemplate>(
                TemplatePath(serverKey, templateId),
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task<JsonApiResource<CustomTemplate>> CreateAsync(
            string serverKey,
            CreateCustomTemplateRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));
            return PostResourceAsync<CustomTemplate, CreateCustomTemplateRequest>(
                TemplatesPath(serverKey),
                ResourceTypes.CustomTemplates,
                request,
                options,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string serverKey,
            string templateId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Connection.DeleteAsync(
                TemplatePath(serverKey, templateId),
                options,
                cancellationToken
            );
        }

        private static string TemplatesPath(string serverKey)
        {
            return CloudServerPath(serverKey) + "/custom_templates";
        }

        private static string TemplatePath(string serverKey, string templateId)
        {
            return TemplatesPath(serverKey) + "/" + Segment(templateId, nameof(templateId));
        }
    }
}
