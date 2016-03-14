using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;

namespace SimpleIdentity.Models
{
    public class UserManager : IUserManager
    {
        private ISimpleIdentityDbContext _dbContext;

        public UserManager(ISimpleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(User user)
        {
            if (user == null) throw new SimpleIdentityDataException("User object is missing");
            if (user.Name == null || user.Name.Length == 0) throw new SimpleIdentityDataException("User.Name is missing");
            if (user.Email == null || user.Email.Length == 0) throw new SimpleIdentityDataException("User.Email is missing");
            user.Authentication = new UserAuthentication { User = user, Secret = Membership.GeneratePassword(8, 0) };
            AuthorizationManager auth = new AuthorizationManager(_dbContext);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            await auth.GrantAuthorizationToUser(Authorization.AuthResourceType.User, user.Id, Authorization.AuthScopes.Full, user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> FindUser(int userId)
        {
            var q = await (from b in _dbContext.Users
                           where b.Id == userId
                           select b).ToListAsync();
            if (!q.Any()) throw new SimpleIdentityDataNotFoundException("User [" + userId + "] cannot be found");
            return q.First();
        }

        public async Task<User> FindUserByName(string userName)
        {
            var q = await (from b in _dbContext.Users
                           where b.Name == userName
                           select b).ToListAsync();
            if (!q.Any()) throw new SimpleIdentityDataNotFoundException("User [" + userName + "] cannot be found");
            return q.First();
        }

        public async Task<User> FindUserByToken(string accessToken)
        {
            var q = await _dbContext.Tokens
                .Where(b => b.AccessToken == accessToken && 
                    b.TokenTimeout > DateTime.Now && 
                    !b.IsRevoked &&
                    b.User.IsActive)
                .Select(b => new { User = b.User }).ToListAsync();
            if (!q.Any()) throw new SimpleIdentityDataNotFoundException("User with Token [" + accessToken + "] cannot be found");
            return q.First().User;
        }

        public async Task<User> UpdateUser(User user)
        {
            if (user == null) throw new SimpleIdentityDataException("User object is missing");
            if (user.Name == null || user.Name.Length == 0) throw new SimpleIdentityDataException("User.Name is missing");
            if (user.Email == null || user.Email.Length == 0) throw new SimpleIdentityDataException("User.Email is missing");

            User userToUpdate = await FindUser(user.Id);
            
            if (userToUpdate.Name != user.Name) throw new SimpleIdentityDataException("Editing User.Name is not supported");
            userToUpdate.Email = user.Email;
            userToUpdate.IsActive = user.IsActive;

            await _dbContext.SaveChangesAsync();
            return userToUpdate;
        }

        public async Task<string> GenerateNonce(int userId)
        {
            User user = await FindUser(userId);
            UserNonce nonce = new UserNonce {
                User = user,
                IsValid = true,
                Timeout = DateTime.Now.AddSeconds(60),
                Nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            };
            _dbContext.UserNonces.Add(nonce);
            await _dbContext.SaveChangesAsync();
            return nonce.Nonce;
        }

        public async Task<IEnumerable<User>> ListUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }
    }
}
