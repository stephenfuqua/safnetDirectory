using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(safnetDirectory.Startup))]
namespace safnetDirectory
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
