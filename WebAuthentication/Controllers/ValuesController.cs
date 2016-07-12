using System.Web.Http;

namespace WebAuthentication.Controllers
{
    [RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new { Value = RequestContext.Principal.Identity.Name });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Anonymous")]
        public IHttpActionResult Anonymous()
        {
            return Ok(new { Value = 19 });
        }
    }
}
