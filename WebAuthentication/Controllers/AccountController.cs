using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebAuthentication.DataAccess;
using WebAuthentication.Models;
using WebAuthentication.Modules;

namespace WebAuthentication.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly Func<IPasswordHasher> _passwordHasherFactory;
        private readonly Func<IUserRepository> _userRepositoryFactory;
        private readonly Func<IUserSessionRepository> _userSessionRepositoryFactory;

        public AccountController(
            Func<IPasswordHasher> passwordHasherFactory,
            Func<IUserRepository> userRepositoryFactory,
            Func<IUserSessionRepository> userSessionRepositoryFactory)
        {
            _passwordHasherFactory = passwordHasherFactory;
            _userRepositoryFactory = userRepositoryFactory;
            _userSessionRepositoryFactory = userSessionRepositoryFactory;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterAsync(RegisterModel model, CancellationToken cancellationToken)
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

            var userRepository = _userRepositoryFactory();

            try
            {
                await userRepository.Insert(user, cancellationToken);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("User already exists.");
            }

            return Ok();
        }

        [HttpGet]
        [Route("Token")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> TokenAsync([FromUri]RegisterModel model, CancellationToken cancellationToken)
        {
            var userRepository = _userRepositoryFactory();

            var user = await userRepository.GetByUserNameAsync(model.UserName, cancellationToken);

            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var passwordHasher = _passwordHasherFactory();

            if (!passwordHasher.Verify(model.Password, user.Password))
            {
                return BadRequest("Invalid username or password.");
            }

            var userSessionRepository = _userSessionRepositoryFactory();

            var userSession = await userSessionRepository.GetByUserIdAsync(user.Id, cancellationToken);

            if (userSession == null)
            {
                userSession = new UserSession
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = Guid.NewGuid().ToString(),
                };

                await userSessionRepository.InsertAsync(userSession, cancellationToken);
            }

            return Ok(userSession.Token);
        }
    }
}
