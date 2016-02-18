using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public interface ITokenManager
    {
        Task<Token> GenerateTokenByPassword(int userId, string userSecret, int clientId, string clientSecret);
        Task<Token> RefreshToken(string accessToken, string refreshToken);
    }
}
