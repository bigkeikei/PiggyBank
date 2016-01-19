using System.Collections.Generic;

namespace PiggyBank.Models
{
    public interface IUserRepository
    {
        User Create(User user);
        User Find(string name);
        User Update(User user);
        IEnumerable<User> List();
        UserAuthentication GenerateAuthentication(User user);
    }
}
