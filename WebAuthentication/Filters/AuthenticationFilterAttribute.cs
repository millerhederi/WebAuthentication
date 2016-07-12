using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Autofac;
using Autofac.Integration.WebApi;
using WebAuthentication.Core;
using WebAuthentication.Modules;

namespace WebAuthentication.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticationFilterAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple { get; } = false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // Don't try to authenticate anonymous endpoints
            if (IsAnonymousRequest(context))
            {
                return;
            }

            var authorization = context.Request.Headers.Authorization;

            if (authorization == null)
            {
                return;
            }

            if (authorization.Scheme != "TokenAuth")
            {
                return;
            }

            var principal = await AuthenticateAsync(context, authorization.Parameter, cancellationToken);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid or expired token", context.Request);
                return;
            }

            context.Principal = principal;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        private static bool IsAnonymousRequest(HttpAuthenticationContext context)
        {
            var anonymousAttributes = context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>();
            return anonymousAttributes.Any();
        }

        private static async Task<IPrincipal> AuthenticateAsync(HttpAuthenticationContext context, string token, CancellationToken cancellationToken)
        {
            // DI hackery. Autofac has their own IAutofacAuthenticationFilter that can be implemented; however, it does not support
            // async methods. Getting around this by implementing the standard IAuthenticationFilter and manually grabbing the container
            // to do service location - https://github.com/autofac/Autofac.WebApi/issues/5
            var config = context.ActionContext.ControllerContext.Configuration;
            var scope = config.DependencyResolver.GetRootLifetimeScope();
            var userSessionRepository = scope.Resolve<IUserSessionRepository>();

            var userSession = await userSessionRepository.GetByTokenAsync(token, cancellationToken);

            if (userSession == null)
            {
                return null;
            }

            var userRepository = scope.Resolve<IUserRepository>();

            var user = await userRepository.GetAsync(userSession.UserId, cancellationToken);

            return new Principal(user.UserName);
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        private readonly string _reason;
        private readonly HttpRequestMessage _request;

        public AuthenticationFailureResult(string reason, HttpRequestMessage request)
        {
            _reason = reason;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = _request,
                ReasonPhrase = _reason
            };

            return Task.FromResult(response);
        }
    }
}