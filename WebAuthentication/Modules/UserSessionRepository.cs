using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAuthentication.DataAccess;

namespace WebAuthentication.Modules
{
    public interface IUserSessionRepository
    {
        Task<UserSession> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<UserSession> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default(CancellationToken));

        Task InsertAsync(
            UserSession userSession,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly ConcurrentDictionary<Guid, UserSession> _userSessions = new ConcurrentDictionary<Guid, UserSession>();

        public Task<UserSession> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            UserSession userSession;
            _userSessions.TryGetValue(userId, out userSession);
            return Task.FromResult(userSession);
        }

        public Task<UserSession> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var userSession = _userSessions.Values.SingleOrDefault(x => x.Token == token);
            return Task.FromResult(userSession);
        }

        public Task InsertAsync(
            UserSession userSession,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_userSessions.TryAdd(userSession.UserId, userSession))
            {
                throw new InvalidOperationException("A user session already exists for the specified user");
            }

            return Task.FromResult(0);
        }
    }
}