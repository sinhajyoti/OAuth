using System.Web.Http;

namespace MVCNet.Portal.Auth.Controllers
{
    [Authorize]
    public class ValidateController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(200);
        }
    }
}