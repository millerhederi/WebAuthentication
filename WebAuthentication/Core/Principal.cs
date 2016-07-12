using System;
using System.Security.Principal;

namespace WebAuthentication.Core
{
    public class Principal : IPrincipal
    {
        public Principal(string userName)
        {
            Identity = new GenericIdentity(userName);
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public IIdentity Identity { get; private set; }
    }
}