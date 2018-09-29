using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        private readonly AppDbContext _context;

        public UserStore(AppDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var userFromDb = await _context.FindAsync<User>(user.Id);

            userFromDb.UserName = user.UserName;
            userFromDb.NormalizedUserName = user.NormalizedUserName;
            userFromDb.PasswordHash = user.PasswordHash;
            userFromDb.Role = user.Role;

            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var userFromDb = await _context.Users.FindAsync(user.Id);
            _context.Users.Remove(userFromDb);

            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(userId);

            return user;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName,
                cancellationToken: cancellationToken);

            return user;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
        }
    }
}