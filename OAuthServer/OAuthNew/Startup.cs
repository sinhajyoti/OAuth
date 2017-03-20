using System;
using System.DirectoryServices.AccountManagement;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using MVCNet.Portal.Auth;
using MVCNet.Portal.Auth.DBContext;

[assembly: OwinStartup(typeof (Startup))]

namespace MVCNet.Portal.Auth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new AuthContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);

            // This is the important part, this is how you will configure the PrincipalContext used throughout your app
            //app.CreatePerOwinContext(() => new PrincipalContext(ContextType.Domain));

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            // token generation
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                // for demo purposes
#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(8),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            });
            app.UseCors(CorsOptions.AllowAll);
            // token consumption
            var config = new HttpConfiguration();
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        private static UserManager<IdentityUser> CreateManager(
            IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<AuthContext>());
            var manager = new UserManager<IdentityUser>(userStore);

            return manager;
        }
    }
}