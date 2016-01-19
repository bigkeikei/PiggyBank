using System.Collections.Generic;

namespace PiggyBank.Models
{
    public interface IPiggyBankRepository
    {
        User CreateUser(User user);
        User FindUser(string name);
        User UpdateUser(User user);
        IEnumerable<User> ListUsers();
        UserAuthentication GenerateAuthentication(User user);
    }
}
