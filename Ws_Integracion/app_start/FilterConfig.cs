using System.Web;
using System.Web.Mvc;

namespace Ws_GIntegracionBus
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
