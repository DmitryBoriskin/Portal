using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ForEample.Startup))]
namespace ForEample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
