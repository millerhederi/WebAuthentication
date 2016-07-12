using System.Web.Http;

namespace WebAuthentication.Controllers
{
    [RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new { Value = 10 });
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
