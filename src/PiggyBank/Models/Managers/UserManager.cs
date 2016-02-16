using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

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
            user.Authentication = new UserAuthentication { User = user, Secret = Membership.GeneratePassword(8, 0) };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> FindUser(int userId)
        {
            var q = await (from b in _dbContext.Users
                           where b.Id == userId
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("User [" + userId + "] cannot be found");
            return q.First();
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
            var q = await (from b in _dbContext.Tokens
                           where b.AccessToken == accessToken &&
                           b.ResourceType == Token.TokenResourceType.User &&
                           b.Scopes == Token.TokenScopes.Full &&
                           b.User.Id == b.ResourceId
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Token [" + accessToken + "] cannot be found");
            int userId = q.First().ResourceId;
            return await FindUser(userId);
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
            if (userToUpdate.Authentication == null)
            {
                userToUpdate.Authentication = new UserAuthentication { User = userToUpdate };
            }
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

            Token token;
            var q = await (from b in _dbContext.Tokens
                           where b.User.Id == userId &&
                           b.ResourceType == Token.TokenResourceType.User &&
                           b.Scopes == Token.TokenScopes.Full
                           select b).ToListAsync();

            if (q.Any())
            {
                token = q.First();
            }
            else
            {
                token = new Token();
                token.User = userToUpdate;
                token.ResourceType = Token.TokenResourceType.User;
                token.Scopes = Token.TokenScopes.Full;
                userToUpdate.Tokens.Add(token);
            }

            token.AccessToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            token.RefreshToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            token.TokenTimeout = System.DateTime.Now.AddMinutes(30);
            await _dbContext.SaveChangesAsync();
            return userToUpdate.Authentication;
        }

        public async Task<bool> CheckAccessToken(string accessToken, Token.TokenResourceType resourceType, int resourceId, Token.TokenScopes scopes)
        {
            var q = await (from b in _dbContext.Tokens
                           where b.AccessToken == accessToken &&
                           b.ResourceType == resourceType &&
                           b.ResourceId == resourceId &&
                           b.Scopes.HasFlag(scopes) &&
                           b.TokenTimeout > DateTime.Now
                           select b).ToListAsync();
            if (!q.Any()) { return false; }
            return true;
        }

        private string Hash(string content)
        {
            MD5 md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(content)));
        }
    }
}
