using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ForExample.Startup))]
namespace ForExample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
