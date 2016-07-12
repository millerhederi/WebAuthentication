using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAuthentication.DataAccess;

namespace WebAuthentication.Modules
{
    public interface IUserRepository
    {
        Task<User> GetAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken));

        Task InsertAsync(User user, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new ConcurrentDictionary<Guid, User>();

        public Task<User> GetAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            User user;
            _users.TryGetValue(id, out user);
            return Task.FromResult(user);
        }

        public Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = _users.Values.SingleOrDefault(x => x.UserName == userName);
            return Task.FromResult(user);
        }

        public async Task InsertAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existingUser = await GetByUserNameAsync(user.UserName, cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Attempting to insert a user with a duplicate user name.");
            }

            if (!_users.TryAdd(user.Id, user))
            {
                throw new InvalidOperationException($"A user with ID '{user.Id}' already exists.");
            }
        }
    }
}