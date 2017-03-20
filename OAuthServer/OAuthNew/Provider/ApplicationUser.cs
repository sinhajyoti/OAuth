using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Payspan.Portal.Auth.DBContext;

namespace Payspan.Portal.Auth
{
    public class ApplicationUserManager : IdentityUser
    {
        private readonly PrincipalContext _context;
        public ApplicationUserManager(IUserStore<IdentityUser> store, PrincipalContext context)
            : base(store)
        {
            _context = context;
        }

        public override async Task<bool> CheckPasswordAsync(ApplicationUserManager user, string password)
        {
            //if (user.IsADUser)
            //{
            //    return await Task.FromResult(_context.ValidateCredentials(user.UserName, password, ContextOptions.Negotiate));
            //}
            //return await base.CheckPasswordAsync(user, password);

            return await Task.FromResult(_context.ValidateCredentials(user.UserName, password, ContextOptions.Negotiate));
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<IdentityUser>(context.Get<AuthContext>()), context.Get<PrincipalContext>());
            //...SNIP...    
        }
    }
}