using System.Web.Mvc;

namespace Ws_Restaurante.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Front()
        {
            return Redirect("~/front/index.html");
        }

        public ActionResult Index()
        {
            return Content("API funcionando");
        }
    }
}