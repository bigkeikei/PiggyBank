using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PiggyBank.Models.Data
{
    public class EFUserManager : IUserManager
    {
        private PiggyBankDbContext _dbContext;

        public EFUserManager(PiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User CreateUser(User user)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            PiggyBankUtility.CheckMandatory(user);
            User userCreated = _dbContext.Users.Add(user);
            userCreated.Authentication = new UserAuthentication();
            _dbContext.SaveChanges();
            return userCreated;
        }

        public User FindUser(int userId)
        {
            return _dbContext.Users.Find(userId);
        }

        public User FindUserByName(string userName)
        {
            return _dbContext.Users.Where(b => b.Name == userName).First();
        }

        public User FindUserByToken(string accessToken)
        {
            return _dbContext.Users.Where(b => b.Authentication.AccessToken == accessToken).First();
        }

        public User UpdateUser(User user)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            User userToUpdate = FindUser(user.Id);

            if (userToUpdate == null) throw new PiggyBankDataException("User [" + user.Id + "] cannot be found");
            if (userToUpdate.Name != user.Name) throw new PiggyBankDataException("Editing User.Name is not supported");
            PiggyBankUtility.CheckMandatory(user);
            PiggyBankUtility.UpdateModel(userToUpdate, user);
            _dbContext.SaveChanges();
            return userToUpdate;
        }

        public IEnumerable<User> ListUsers()
        {
            return _dbContext.Users;
        }

        public UserAuthentication GenerateChallenge(int userId)
        {
            User userToUpdate = FindUser(userId);
            if (userToUpdate == null) throw new PiggyBankDataException("User [" + userId + "] cannot be found");

            userToUpdate.Authentication.Challenge = userToUpdate.Name + System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            userToUpdate.Authentication.AccessToken = null;
            userToUpdate.Authentication.RefreshToken = null;
            userToUpdate.Authentication.TokenTimeout = null;
            _dbContext.SaveChanges();
            return userToUpdate.Authentication;
        }

        public UserAuthentication GenerateToken(int userId)
        {
            User userToUpdate = FindUser(userId);
            if (userToUpdate == null) throw new PiggyBankDataException("User [" + userId + "] cannot be found");

            userToUpdate.Authentication.AccessToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            userToUpdate.Authentication.RefreshToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            userToUpdate.Authentication.TokenTimeout = System.DateTime.Now.AddMinutes(30);
            _dbContext.SaveChanges();
            return userToUpdate.Authentication;
        }

        private string Hash(string content)
        {
            MD5 md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(content)));
        }
    }
}
