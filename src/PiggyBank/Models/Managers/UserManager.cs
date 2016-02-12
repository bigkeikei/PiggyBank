using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public class UserManager : IUserManager
    {
        private IPiggyBankDbContext _dbContext;

        public UserManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(User user)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            PiggyBankUtility.CheckMandatory(user);
            user.Authentication = new UserAuthentication();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> FindUser(int userId)
        {
            User user = await _dbContext.Users.FindAsync(userId);
            if ( user == null ) throw new PiggyBankDataNotFoundException("User [" + userId + "] cannot be found");
            return user;
        }

        public async Task<User> FindUserByName(string userName)
        {
            var q = await (from b in _dbContext.Users
                           where b.Name == userName
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("User [" + userName + "] cannot be found");
            return q.First();
        }

        public async Task<User> FindUserByToken(string accessToken)
        {
            var q = await (from b in _dbContext.Users
                           where b.Authentication.AccessToken == accessToken
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("User with token [" + accessToken + "] cannot be found");
            return q.First();
        }

        public async Task<User> UpdateUser(User user)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            User userToUpdate = await FindUser(user.Id);
            
            if (userToUpdate.Name != user.Name) throw new PiggyBankDataException("Editing User.Name is not supported");
            PiggyBankUtility.CheckMandatory(user);
            PiggyBankUtility.UpdateModel(userToUpdate, user);
            await _dbContext.SaveChangesAsync();
            return userToUpdate;
        }

        public async Task<IEnumerable<User>> ListUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<UserAuthentication> GenerateChallenge(int userId)
        {
            User userToUpdate = await FindUser(userId);
            userToUpdate.Authentication.Challenge = userToUpdate.Name + System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            userToUpdate.Authentication.ChallengeTimeout = System.DateTime.Now.AddSeconds(60);

            await _dbContext.SaveChangesAsync();
            return userToUpdate.Authentication;
        }

        public async Task<UserAuthentication> GenerateToken(int userId, string signature)
        {
            User userToUpdate = await FindUser(userId);
            UserAuthentication auth = userToUpdate.Authentication;
            MD5 md5 = MD5.Create();
            string authSign = Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(auth.Challenge + auth.Secret)));
            if (authSign != signature) { throw new PiggyBankDataException("Invalid signature [" + signature + "]"); }
            if (DateTime.Now >= auth.ChallengeTimeout) { throw new PiggyBankAuthenticationTimeoutException("Challenge expired"); }

            userToUpdate.Authentication.AccessToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            userToUpdate.Authentication.RefreshToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            userToUpdate.Authentication.TokenTimeout = System.DateTime.Now.AddMinutes(30);
            await _dbContext.SaveChangesAsync();
            return userToUpdate.Authentication;
        }

        public async Task<User> CheckAccessToken(int userId, string accessToken)
        {
            User user = await FindUser(userId);
            if (user.Authentication.AccessToken != accessToken) { throw new PiggyBankDataException("Invalid token [" + accessToken + "]"); }
            if (DateTime.Now >= user.Authentication.TokenTimeout) { throw new PiggyBankAuthenticationTimeoutException("Token expired"); }
            return user;
        }

        private string Hash(string content)
        {
            MD5 md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(content)));
        }
    }
}
