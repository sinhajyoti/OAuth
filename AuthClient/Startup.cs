using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AuthClient.Startup))]
namespace AuthClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
