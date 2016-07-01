using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApi.Startup))]
namespace WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
