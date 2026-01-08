using System.Web.Http;
using System.Web.Http.Cors;

namespace Ws_GIntegracionBus.Middlewares
{
    public static class CorsMiddleware
    {
        public static void EnableCors(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
        }
    }
}
