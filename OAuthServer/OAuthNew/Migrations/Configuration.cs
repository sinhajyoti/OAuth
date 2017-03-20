using System;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCNet.Portal.Auth.DBContext;
using MVCNet.Portal.Auth.Models;

namespace OAuthNew.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AuthContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AuthContext context)
        {
            context.Clients.AddOrUpdate(
                client => client.Name,
                new Client
                {
                    Id = "42ff5dad3c274c97a3a7c3d44b67bb42",
                    Name = "Demo Resource Owner Password Credentials Grant Client",
                    ClientSecretHash = new PasswordHasher().HashPassword("secret"),
                    AllowedGrant = OAuthGrant.ResourceOwner,
                    CreatedOn = DateTimeOffset.UtcNow
                });

            context.Users.AddOrUpdate(
                user => user.UserName,
                new IdentityUser("Jyoti")
                {
                    Id = Guid.NewGuid().ToString("N"),
                    PasswordHash = new PasswordHasher().HashPassword("test123"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = "jyoti.sinha@live.com",
                    EmailConfirmed = true
                });
        }
    }
}