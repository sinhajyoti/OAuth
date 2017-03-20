using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCNet.Portal.Auth.Models;

namespace MVCNet.Portal.Auth.DBContext
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<IdentityClaim> IdentityClaims { get; set; }
    }
}