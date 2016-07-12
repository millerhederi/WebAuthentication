using System.Web.Http;
using WebAuthentication.Filters;

namespace WebAuthentication.Controllers
{
    [AuthenticationFilter]
    public class ValuesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new { Value = 10 });
        }
    }
}
