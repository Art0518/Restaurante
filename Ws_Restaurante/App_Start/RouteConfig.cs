using System.Web.Mvc;
using System.Web.Routing;

namespace Ws_Restaurante
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // 👉 Redirige la raíz al front/index.html
            routes.MapRoute(
                name: "Front",
                url: "",
                defaults: new { controller = "Home", action = "Front" }
            );

            // 👉 Rutas MVC normales
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
