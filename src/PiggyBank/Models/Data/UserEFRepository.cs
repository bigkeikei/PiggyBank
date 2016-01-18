using System.Collections.Generic;
using System.Linq;

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
            _dbContext.SaveChanges();
            return userToCreate;
        }

        public User Find(string name)
        {
            return _dbContext.Users.Where(b => b.Name == name).First();
        }

        public User Update(User user)
        {
            if (user == null)
            {
                return null;
            }
            User userToUpdate = _dbContext.Users.Find(user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.IsActive = user.IsActive;
                _dbContext.SaveChanges();
            }
            return userToUpdate;
        }

        public IEnumerable<User> List()
        {
            return _dbContext.Users;
        }
    }
}
