using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using WebAuthentication.Core;

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

            var isAuthenticated = await AuthenticateAsync(authorization.Parameter);

            if (isAuthenticated)
            {
                context.Principal = new Principal("miller");
            }
            else
            {
                //context.ErrorResult = new UnauthorizedResult();
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        private bool IsAnonymousRequest(HttpAuthenticationContext context)
        {
            var anonymousAttributes = context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>();
            return anonymousAttributes.Any();
        }

        private static Task<bool> AuthenticateAsync(string parameter)
        {
            var isValid = parameter == "abc";

            return Task.FromResult(isValid);
        }
    }
}