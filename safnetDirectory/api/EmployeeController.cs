using System;
using safnetDirectory.FullMvc.Controllers;
using safnetDirectory.FullMvc.Data;
using safnetDirectory.FullMvc.Models;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Http;

namespace safnetDirectory.FullMvc.api
{
    public class EmployeeController : ApiController
    {
        private readonly IDbContext _dbContext;

        public EmployeeController(IDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException();
        }


        [HttpGet]
        [Authorize(Roles = AdminController.HR_ROLE)]
        public IHttpActionResult Get([FromUri] string id)
        {
            var employee = _dbContext.Users
                .Where(x => x.Id == id)
                .AsEnumerable()
                .Select(Map)
                .FirstOrDefault();

            if (employee == null) return NotFound();

            return Ok(employee);
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult Get([FromUri] EmployeeViewModel searchModel = null, [FromUri] int pageSize = 10, [FromUri] int page = 1)
        {
            searchModel = searchModel ?? new EmployeeViewModel();

            var query = _dbContext.Users;

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


            var users = query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(Map)
                .OrderBy(x => x.name);

            var data = new EmployeeList
            {
                TotalRecords = _dbContext.Users.Count(),
                Employees = users
            };

            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (!claimsIdentity.Claims.Any(x => x.Value == AdminController.HR_ROLE))
            {
                // this user is not an HR user and cannot edit records
                foreach (var x in data.Employees)
                {
                    x.id = string.Empty;
                }
            }


            return Ok(data);
        }



        // TODO: this should be a PUT not a POST
        [HttpPost]
        [Authorize(Roles = AdminController.HR_ROLE)]
        public IHttpActionResult Post([FromBody] EmployeeViewModel model)
        {
            if (model == null) return BadRequest();

            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == model.id);

                if (user == null) return NotFound();

                user.FullName = model.name;
                user.Title = model.title;
                user.Location = model.location;
                user.Email = model.email;
                user.PhoneNumber = model.office;
                user.MobilePhoneNumber = model.mobile;

                _dbContext.SaveChanges();

                return StatusCode(HttpStatusCode.Accepted);
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
