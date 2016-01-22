using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using PiggyBank.Models;
using System;
using System.Text;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockUserManager : IUserManager
    {
        private MockPiggyBankDbContext _dbContext;
        private int _userId;

        public MockUserManager(MockPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
            _userId = 0;
        }
        public IEnumerable<User> ListUsers()
        {
            return _dbContext.Users;
        }

        public User CreateUser(User user)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            user.Id = ++_userId;
            user.Authentication = new UserAuthentication();
            user.Authentication.Secret = "secret";
            PiggyBankUtility.CheckMandatory(user);
            _dbContext.Users.Add(user);
            return user;
        }

        public User FindUser(int userId)
        {
            var q = _dbContext.Users.Where(b => b.Id == userId);
            if (!q.Any()) throw new PiggyBankDataNotFoundException("User [" + userId + "] cannot be found");
            return q.First();
        }

        public User FindUserByName(string userName)
        {
            var q = _dbContext.Users.Where(b => b.Name == userName);
            if (!q.Any()) throw new PiggyBankDataNotFoundException("User [" + userName + "] cannot be found");
            return q.First();
        }

        public User FindUserByToken(string accessToken)
        {
            var q = _dbContext.Users.Where(b => b.Authentication.AccessToken == accessToken);
            if (!q.Any()) throw new PiggyBankDataNotFoundException("User with token [" + accessToken + "] cannot be found");
            return q.First();
        }

        public User UpdateUser(User user)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            PiggyBankUtility.CheckMandatory(user);
            User userToUpdate = FindUser(user.Id);
            PiggyBankUtility.UpdateModel(userToUpdate, user);
            return userToUpdate;
        }

        public UserAuthentication GenerateChallenge(int userId)
        {
            User user = FindUser(userId);
            user.Authentication.Challenge = user.Name + System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            user.Authentication.AccessToken = null;
            user.Authentication.RefreshToken = null;
            user.Authentication.TokenTimeout = null;
            return user.Authentication;
        }

        public UserAuthentication GenerateToken(int userId)
        {
            User user = FindUser(userId);
            user.Authentication.AccessToken = Hash(System.Guid.NewGuid().ToString() + user.Name);
            user.Authentication.RefreshToken = Hash(System.Guid.NewGuid().ToString() + user.Name);
            user.Authentication.TokenTimeout = System.DateTime.Now.AddMinutes(30);
            return user.Authentication;
        }

        private string Hash(string content)
        {
            MD5 md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(content)));
        }
    }
}
