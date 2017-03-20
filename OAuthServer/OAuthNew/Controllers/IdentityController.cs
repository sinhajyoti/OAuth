using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace MVCNet.Portal.Auth.Controllers
{
    [Authorize]
    public class IdentityController : ApiController
    {
        public IEnumerable<MVCNet.Portal.Auth.Models.IdentityClaim> Get()
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            if (principal != null)
            {
                return from c in principal.Claims
                       select new MVCNet.Portal.Auth.Models.IdentityClaim
                    {
                        Id=Guid.NewGuid().ToString("N"),
                        Type = c.Type,
                        Value = c.Value
                    };
            }
            return null;
        }
    }
}