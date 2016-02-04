using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Elmah.Extensions.Demo.Startup))]
namespace Elmah.Extensions.Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
