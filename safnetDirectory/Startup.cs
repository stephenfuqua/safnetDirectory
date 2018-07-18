using Microsoft.Owin;
using Owin;
using safnetDirectory.FullMvc.Data;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(safnetDirectory.FullMvc.Startup))]
namespace safnetDirectory.FullMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            HttpConfiguration config = new HttpConfiguration();

            ConfigureApiRouting(config);
            ConfigureDependencyInjection(config);

            app.UseWebApi(config);
        }

        private void ConfigureApiRouting(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private void ConfigureDependencyInjection(HttpConfiguration config)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.Register<IDbContext, ApplicationDbContext>(Lifestyle.Scoped);


            container.RegisterWebApiControllers(config);
            container.Verify();

            config.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}
