using System.Web.Http;
using Ws_GIntegracionBus.Middlewares;

namespace Ws_GIntegracionBus
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Middlewares globales
            config.MessageHandlers.Add(new ErrorMiddleware());
            //config.MessageHandlers.Add(new AuthMiddleware());
            CorsMiddleware.EnableCors(config);

            //  Habilitar rutas por atributo (lo más importante)
            config.MapHttpAttributeRoutes();

            // Ruta por defecto solo para compatibilidad
            // NO interfiere con /api/v1/integracion/restaurantes/...
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // JSON por defecto
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}

