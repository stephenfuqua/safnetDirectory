using safnetDirectory.FullMvc.Controllers;
using safnetDirectory.FullMvc.Models;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;

namespace safnetDirectory.FullMvc.api
{
    public class EmployeeController : ApiController
    {
        [HttpGet]
        [Authorize(Roles = AdminController.HR_ROLE)]
        public IHttpActionResult Get([FromUri] string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var employee = db.Users
                    .Where(x => x.Id == id)
                    .Select(Map)
                    .FirstOrDefault();

                return Ok(employee);
            }
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult Get([FromUri] EmployeeViewModel searchModel, [FromUri] int pageSize = 10, [FromUri] int page = 1)
        {
            
            using (var db = new ApplicationDbContext())
            {
                var query = db.Users.AsQueryable();

                if (searchModel != null)
                {
                    if (!string.IsNullOrWhiteSpace(searchModel.name))
                    {
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
                

                var claimsIdentity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                if (!claimsIdentity.Claims.Any(x => x.Value == AdminController.HR_ROLE))
                {
                    // this user is not an HR user and cannot edit records
                    data.employees.ForEach(x => x.id = string.Empty);
                }


                return Ok(data);
            }
        }

        // TODO: this should be a PUT not a POST
        [HttpPost]
        [Authorize(Roles = AdminController.HR_ROLE)]
        public IHttpActionResult Post([FromBody] EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Models.ApplicationDbContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == model.id);

                    // TODO: null handling
                    
                    user.Id = model.id;
                    user.FullName = model.name;
                    user.Title = model.title;
                    user.Location = model.location;
                    user.Email = model.email;
                    user.PhoneNumber = model.office;
                    user.MobilePhoneNumber = model.mobile;

                    db.SaveChanges();

                    return StatusCode(HttpStatusCode.Accepted);
                }
            }
            return BadRequest(ModelState);
        }

        private EmployeeViewModel Map(ApplicationUser user)
        {
            return new EmployeeViewModel
            {
                id = user.Id,
                name = user.FullName,
                title = user.Title,
                location = user.Location,
                email = user.Email,
                office = user.PhoneNumber,
                mobile = user.MobilePhoneNumber
            };
        }

    }
}
