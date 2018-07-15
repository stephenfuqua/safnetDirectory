using System.Web.Mvc;

namespace safnetDirectory.FullMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize(Roles = AdminController.USER_ROLE)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = AdminController.HR_ROLE)]
        public ActionResult Edit()
        {

            return View();
        }


        [Authorize(Roles = AdminController.USER_ROLE)]
        public ActionResult Employees()
        {
            return View();
        }
    }

}