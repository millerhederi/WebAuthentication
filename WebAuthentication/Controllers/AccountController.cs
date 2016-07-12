using System;
using System.Collections.Concurrent;
using System.Web.Http;
using WebAuthentication.DataAccess;
using WebAuthentication.Models;
using WebAuthentication.Services;

namespace WebAuthentication.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

        private readonly Func<IPasswordHasher> _passwordHasherFactory;

        public AccountController(Func<IPasswordHasher> passwordHasherFactory)
        {
            _passwordHasherFactory = passwordHasherFactory;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public IHttpActionResult Register(RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var passwordHasher = _passwordHasherFactory();

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = model.UserName,
                Password = passwordHasher.HashPassword(model.Password),
            };

            if (!Users.TryAdd(model.UserName, user))
            {
                return BadRequest("User already exists.");
            }

            return Ok();
        }

        [HttpGet]
        [Route("Token")]
        [AllowAnonymous]
        public IHttpActionResult Token([FromUri]RegisterModel model)
        {
            User user;

            if (!Users.TryGetValue(model.UserName, out user))
            {
                return BadRequest("Invalid username or password.");
            }

            var passwordHasher = _passwordHasherFactory();

            if (!passwordHasher.Verify(model.Password, user.Password))
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(Guid.NewGuid());
        }
    }
}
