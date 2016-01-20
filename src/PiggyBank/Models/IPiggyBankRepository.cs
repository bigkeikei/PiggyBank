using System.Collections.Generic;

namespace PiggyBank.Models
{
    public interface IPiggyBankRepository
    {
        // User Maintenance
        IEnumerable<User> ListUsers();
        User CreateUser(User user);
        User FindUser(int userId);
        User FindUserByName(string userName);
        User FindUserByToken(string accessToken);
        User UpdateUser(User user);
        UserAuthentication GenerateChallenge(int userId);
        UserAuthentication GenerateToken(int userId);

        // Book Maintenance
        Book CreateBook(User user, Book book);
        Book FindBook(int userId, int bookId);
        Book UpdateBook(int userId, Book book);
    }
}
