using System.Web.Http;
using System.Web.Http.Cors;

namespace Ws_Restaurante
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // --- CORS GLOBAL (OBLIGATORIO EN MONSTERASP) ---
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // RUTAS API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
