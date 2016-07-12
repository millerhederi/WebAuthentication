using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WebAuthentication.DataAccess;

namespace WebAuthentication.Modules
{
    public interface IUserRepository
    {
        Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken));

        Task Insert(User user, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        public Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            User user;
            _users.TryGetValue(userName, out user);
            return Task.FromResult(user);
        }

        public Task Insert(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_users.TryAdd(user.UserName, user))
            {
                throw new InvalidOperationException("Attempting to insert a duplicate user.");
            }

            return Task.FromResult(0);
        }
    }
}