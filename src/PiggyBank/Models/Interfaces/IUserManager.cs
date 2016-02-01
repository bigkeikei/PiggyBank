using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IUserManager
    {
        IEnumerable<User> ListUsers();
        User CreateUser(User user);
        User FindUser(int userId);
        User FindUserByName(string userName);
        User FindUserByToken(string accessToken);
        User UpdateUser(User user);
        UserAuthentication GenerateChallenge(int userId);
        UserAuthentication GenerateToken(int userId, string signature);
        User CheckAccessToken(int userId, string accessToken);
    }
}
