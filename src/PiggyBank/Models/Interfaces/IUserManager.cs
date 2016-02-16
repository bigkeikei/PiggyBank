using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IUserManager
    {
        Task<IEnumerable<User>> ListUsers();
        Task<User> CreateUser(User user);
        Task<User> FindUser(int userId);
        Task<User> FindUserByName(string userName);
        Task<User> FindUserByToken(string accessToken);
        Task<User> UpdateUser(User user);
        Task<UserAuthentication> GenerateChallenge(int userId);
        Task<UserAuthentication> GenerateToken(int userId, string signature);
        Task<bool> CheckAccessToken(string accessToken, Token.TokenResourceType resourceType, int resourceId, Token.TokenScopes scopes);
    }
}
