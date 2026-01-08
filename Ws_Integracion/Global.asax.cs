using System.Web;
using System.Web.Http;
using Ws_GIntegracionBus.App_Start;

namespace Ws_GIntegracionBus
{

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(SwaggerConfig.Register);

        }
    }
}
