using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace WebAuthentication.Filters
{
    public class AuthenticationFilterAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple { get; } = true;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // Don't try to authenticate anonymous endpoints
            if (IsAnonymousRequest(context))
            {
                return Task.FromResult(0);
            }

            var authorization = context.Request.Headers.Authorization;

            if (authorization == null)
            {
                return null;
            }

            if (authorization.Scheme != "TokenAuth")
            {
                return null;
            }

            return Task.FromResult(0);
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
    }
}