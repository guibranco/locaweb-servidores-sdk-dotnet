using System.Threading;
using System.Threading.Tasks;
using LocawebServidores.JsonApi;
using LocawebServidores.Models;
using LocawebServidores.Models.Requests;

namespace LocawebServidores.Resources
{
    /// <summary>
    /// Custom template endpoints (<c>/cloud/servers/{serverKey}/custom_templates</c>).
    /// </summary>
    public interface ICustomTemplatesResource
    {
        /// <summary>
        /// Lists the custom templates of a cloud server.
        /// <c>GET /cloud/servers/{serverKey}/custom_templates</c>
        /// </summary>
        Task<ResourceCollection<CustomTemplate>> ListAsync(
            string serverKey,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets the details of a custom template.
        /// <c>GET /cloud/servers/{serverKey}/custom_templates/{templateId}</c>
        /// </summary>
        Task<JsonApiResource<CustomTemplate>> GetAsync(
            string serverKey,
            string templateId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates a custom template from a cloud server.
        /// <c>POST /cloud/servers/{serverKey}/custom_templates</c>
        /// </summary>
        Task<JsonApiResource<CustomTemplate>> CreateAsync(
            string serverKey,
            CreateCustomTemplateRequest request,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes a custom template.
        /// <c>DELETE /cloud/servers/{serverKey}/custom_templates/{templateId}</c>
        /// </summary>
        Task DeleteAsync(
            string serverKey,
            string templateId,
            RequestOptions? options = null,
            CancellationToken cancellationToken = default
        );
    }
}
