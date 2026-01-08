using Swashbuckle.Swagger;
using System.Web;

namespace Ws_GIntegracionBus.App_Start.Swagger
{
    public class SwaggerBasePathFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, System.Web.Http.Description.IApiExplorer apiExplorer)
        {
            // Obtener la request actual
            var request = HttpContext.Current?.Request;

            if (request == null)
                return;

            // Ejemplo: localhost:44301
            swaggerDoc.host = request.Url.Authority;

            // http o https
            swaggerDoc.schemes = new[] { request.Url.Scheme };

            // Ruta base (root del API)
            swaggerDoc.basePath = "/";
        }
    }
}
