using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
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

            var principal = await AuthenticateAsync(authorization.Parameter);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", context.Request);
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

        private static Task<IPrincipal> AuthenticateAsync(string parameter)
        {
            var isValid = parameter == "abc";

            if (!isValid)
            {
                return null;
            }

            IPrincipal principal = new Principal("miller");
            return Task.FromResult(principal);
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