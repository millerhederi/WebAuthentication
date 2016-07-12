using System;

namespace WebAuthentication.DataAccess
{
    public class UserSession
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; }
    }
}