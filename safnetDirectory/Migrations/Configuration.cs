using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using safnetDirectory.FullMvc.Controllers;
using safnetDirectory.FullMvc.Data;
using safnetDirectory.FullMvc.Models;
using System.Data.Entity.Migrations;

namespace safnetDirectory.FullMvc.Migrations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "safnetDirectory.FullMvc.Data.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Configuration.LazyLoadingEnabled = true;

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists(AdminController.HR_ROLE))
            {
                roleManager.Create(new IdentityRole(AdminController.HR_ROLE));
            }
            if (!roleManager.RoleExists(AdminController.USER_ROLE))
            {
                roleManager.Create(new IdentityRole(AdminController.USER_ROLE));
            }

            var user = new ApplicationUser { UserName = "sysadmin@prism.com" };


            if (userManager.FindByName("sysadmin@prism.com") == null)
            {
                var result = userManager.Create(user, "Sys.admin9");

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, AdminController.HR_ROLE);
                    userManager.AddToRole(user.Id, AdminController.USER_ROLE);
                }
            }

            context.SaveChanges();
        }


    }
}
