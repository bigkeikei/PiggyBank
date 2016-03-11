using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace SimpleIdentity.Models
{
    public interface ITokenManager
    {
        Task<Token> GenerateTokenBySignature(int userId, int clientId, string signature);
        Task<Token> GenerateTokenByPassword(int userId, string userSecret, int clientId, string clientSecret);
        Task<Token> RefreshToken(string accessToken, string refreshToken);
        Task<string> ComputeSignature(string accessToken, string method, string url, Stream body = null, Dictionary<string, string> parameters = null);
    }
}
