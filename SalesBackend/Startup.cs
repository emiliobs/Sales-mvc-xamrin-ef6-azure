using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SalesBackend.Startup))]
namespace SalesBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
