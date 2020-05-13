using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Management_application.Startup))]
namespace Management_application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
