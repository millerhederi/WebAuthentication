using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebAuthentication.Core;

namespace WebAuthentication.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.Request.GetRequestContext().Principal as Principal;

            if (principal == null)
            {
                return false;
            }

            return true;
        }
    }
} 