using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Newtonsoft.Json;
using safnetDirectory.FullMvc.Models;
using System.Net;

namespace safnetDirectory.FullMvc.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult Edit()
        {          

            return View();
        }

        public JsonResult GetRecord(string id)
        {
            using (var db = new Models.ApplicationDbContext())
            {
                var employee = db.Users
                    .Where(x => x.Id == id)
                    .Select(x => new EmployeeViewModel { id = x.Id, name = x.FullName, title = x.Title, location = x.Location, email = x.Email, office = x.PhoneNumber, mobile = x.MobilePhoneNumber })
                    .FirstOrDefault();

                return Json(employee, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public HttpStatusCodeResult Edit(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Models.ApplicationDbContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == model.id);
                    user.Id = model.id;
                    user.FullName = model.name;
                    user.Title = model.title;
                    user.Location = model.location;
                    user.Email = model.email;
                    user.PhoneNumber = model.office;
                    user.MobilePhoneNumber = model.mobile;

                    db.SaveChanges();

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ModelState.ToString());
        }


        public ActionResult Employees()
        {
            return View();
        }


        public JsonResult EmployeePaging(int pageSize = 10, int page = 1, string searchText = "")
        {

            var searchModel = JsonConvert.DeserializeObject<EmployeeViewModel>(searchText);

            using (var db = new Models.ApplicationDbContext())
            {
                var query = db.Users.AsQueryable();

                if (searchModel != null)
                {
                    if (!string.IsNullOrWhiteSpace(searchModel.name))
                    {
                        // TODO: injection protection
                        query = query.Where(x => x.FullName.Contains(searchModel.name));
                    }
                    if (!string.IsNullOrWhiteSpace(searchModel.title))
                    {
                        query = query.Where(x => x.Title.Contains(searchModel.title));
                    }
                    if (!string.IsNullOrWhiteSpace(searchModel.location))
                    {
                        query = query.Where(x => x.Location.Contains(searchModel.location));
                    }
                    if (!string.IsNullOrWhiteSpace(searchModel.email))
                    {
                        query = query.Where(x => x.Email.Contains(searchModel.email));
                    }
                }

                var users = query
                    .OrderBy(x => x.FullName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new EmployeeViewModel { id = x.Id, name = x.FullName, title = x.Title, location = x.Location, email = x.Email, office = x.PhoneNumber, mobile = x.MobilePhoneNumber })
                    .ToList();

                var data = new
                {
                    employees = users.ToList(),
                    totalRecords = query.Count()
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

    }

}