using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public interface ITokenManager
    {
        Task<string> GenerateNonce(int userId);
        Task<Token> GenerateTokenBySignature(int userId, int clientId, string sign);
        Task<Token> GenerateTokenByPassword(int userId, string userSecret, int clientId, string clientSecret);
        Task<Token> RefreshToken(string accessToken, string refreshToken);
    }
}
