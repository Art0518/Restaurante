using System.Linq;
using System.Web.Http;
using Swashbuckle.Application;
using Ws_GIntegracionBus.App_Start.Swagger;

namespace Ws_GIntegracionBus.App_Start
{
    public class SwaggerConfig
    {
        // ✔ FIRMA CORRECTA PARA USAR CON GlobalConfiguration.Configure()
        public static void Register(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                // 🟦 Nombre oficial de tu API
                c.SingleApiVersion("v1", "UnRinconEnSanJuan - Bus Integración REST");

                // 🟦 Rutas por atributo
                c.UseFullTypeNameInSchemaIds();
                c.DocumentFilter<SwaggerBasePathFilter>();
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                // 🟦 Enums como cadenas
                c.DescribeAllEnumsAsStrings();

                // 🟦 Comentarios XML (si existe)
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                var xmlPath = System.IO.Path.Combine(baseDirectory, "Ws_GIntegracionBus.XML");
                if (System.IO.File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // 🟦 HATEOAS
                c.SchemaFilter<SwaggerHateoasFilter>();

            })
            .EnableSwaggerUi(c =>
            {
                c.DocumentTitle("UnRinconEnSanJuan - API REST v1 (Bus Integración)");
                c.EnableDiscoveryUrlSelector();
                c.InjectStylesheet(typeof(SwaggerConfig).Assembly, "Ws_GIntegracionBus.SwaggerTheme.css");
            });
        }
    }
}
