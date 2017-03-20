using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using MVCNet.Portal.Auth.DBContext;
using MVCNet.Portal.Auth.Models;

namespace MVCNet.Portal.Auth
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication
            (OAuthValidateClientAuthenticationContext context)
        {
            // validate client credentials (demo)
            // should be stored securely (salted, hashed, iterated)
            string clientKey, clientSecret;
            if (context.TryGetBasicCredentials(out clientKey, out clientSecret))
            {
                try
                {
                    var userManager =
                        context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
                    var dbContext = context.OwinContext.Get<AuthContext>();

                    Client client =
                        await dbContext.Clients.FirstOrDefaultAsync(clientEntity => clientEntity.Id == clientKey);

                    if (client != null
                        &&
                        userManager.PasswordHasher.VerifyHashedPassword(client.ClientSecretHash, clientSecret) ==
                        PasswordVerificationResult.Success
                        )
                    {
                        // Client has been verified.
                        context.OwinContext.Set("oauth:client", client);
                        context.Validated(clientKey);
                    }
                    else
                    {
                        // Client could not be validated.
                        context.SetError("invalid_client", "Client credentials are invalid.");
                        context.Rejected();
                    }
                }
                catch
                {
                    // Could not get the client through the IClientManager implementation.
                    context.SetError("server_error");
                    context.Rejected();
                }
            }
            else
            {
                // The client credentials could not be retrieved.
                context.SetError("invalid_client",
                    "Client credentials could not be retrieved through the Authorization header.");

                context.Rejected();
            }
        }

        public override async Task GrantResourceOwnerCredentials
            (OAuthGrantResourceOwnerCredentialsContext context)
        {
            var client = context.OwinContext.Get<Client>("oauth:client");
            if (client.AllowedGrant == OAuthGrant.ResourceOwner)
            {
                // Client flow matches the requested flow. Continue...
                var userManager =
                    context.OwinContext.GetUserManager<UserManager<IdentityUser>>();

                IdentityUser user;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch
                {
                    // Could not retrieve the user.
                    context.SetError("server_error");
                    context.Rejected();

                    // Return here so that we don't process further. Not ideal but needed to be done here.
                    return;
                }

                if (user != null)
                {
                    try
                    {
                        // User is found. Signal this by calling context.Validated
                        ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                            user,
                            DefaultAuthenticationTypes.ExternalBearer);
                        // add custom claims to identity
                        identity.AddClaim(new Claim("sub", context.UserName));
                        identity.AddClaim(new Claim("role", "user"));
                        identity.AddClaim(new Claim("privileges", "Admin,AccountViewer,AccountSubmit"));

                        var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {"client_key", context.ClientId}
                            });

                        var ticket = new AuthenticationTicket(identity, props);
                        context.Validated(ticket);

                        //context.Validated(identity);
                    }
                    catch
                    {
                        // The ClaimsIdentity could not be created by the UserManager.
                        context.SetError("server_error");
                        context.Rejected();
                    }
                }
                else
                {
                    // The resource owner credentials are invalid or resource owner does not exist.
                    context.SetError(
                        "access_denied",
                        "The resource owner credentials are invalid or resource owner does not exist.");

                    context.Rejected();
                }
            }
            else
            {
                // Client is not allowed for the 'Resource Owner Password Credentials Grant'.
                context.SetError(
                    "invalid_grant",
                    "Client is not allowed for the 'Resource Owner Password Credentials Grant'");

                context.Rejected();
            }
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClientKey = context.Ticket.Properties.Dictionary["client_key"];
            var client = context.OwinContext.Get<Client>("oauth:client");
            var currentClientKey = client.Id;

            // enforce client binding of refresh token
            if (originalClientKey != currentClientKey)
            {
                context.Rejected();
                return;
            }


            // chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            string accessToken = context.AccessToken; //store this to DB
            return base.TokenEndpointResponse(context);
        }
    }
}