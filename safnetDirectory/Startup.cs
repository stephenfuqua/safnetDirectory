using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(safnetDirectory.FullMvc.Startup))]
namespace safnetDirectory.FullMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
