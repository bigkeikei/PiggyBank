using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace PiggyBank.Models.Data
{
    public class UserEFRepository : IUserRepository
    {
        PiggyBankDbContext _dbContext;

        public UserEFRepository(PiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User Create(User user)
        {
            if (user == null)
            {
                return null;
            }
            User userToCreate = _dbContext.Users.Add(user);
            userToCreate.Authentication = new UserAuthentication();
            _dbContext.SaveChanges();
            return userToCreate;
        }

        public User Find(string name)
        {
            return _dbContext.Users.Where(b => b.Name == name).First();
        }

        public User Update(User user)
        {
            User userToUpdate = GetUserToUpdate(user);
            if (userToUpdate == null)
            {
                return null;
            }
            userToUpdate.IsActive = user.IsActive;
            userToUpdate.Email = user.Email;
            _dbContext.SaveChanges();
            return userToUpdate;
        }

        public IEnumerable<User> List()
        {
            return _dbContext.Users;
        }

        public UserAuthentication GenerateAuthentication(User user)
        {
            User userToUpdate = GetUserToUpdate(user);
            if (userToUpdate == null)
            {
                return null;
            }
            userToUpdate.Authentication.Challenge = userToUpdate.Name + System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            userToUpdate.Authentication.AccessToken = System.Guid.NewGuid().ToString();
            userToUpdate.Authentication.RefreshToken = System.Guid.NewGuid().ToString();
            userToUpdate.Authentication.TokenTimeout = System.DateTime.Now.AddMinutes(30);
            _dbContext.SaveChanges();
            return userToUpdate.Authentication;
        }

        private User GetUserToUpdate(User user)
        {
            if (user == null)
            {
                return null;
            }
            User userToUpdate = _dbContext.Users.Find(user.Id);
            if (userToUpdate == null || userToUpdate.Name != user.Name)
            {
                return null;
            }
            return userToUpdate;
        }
    }
}
