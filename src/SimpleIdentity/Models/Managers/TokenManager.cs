using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdentity.Models
{
    public class TokenManager : ITokenManager
    {
        private ISimpleIdentityDbContext _dbContext;

        public TokenManager(ISimpleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateNonce(int userId)
        {
            var auths = await _dbContext.Users
                .Where(b => b.Id == userId &&
                    b.IsActive)
                .Select(b => b.Authentication).ToListAsync();
            if (!auths.Any()) { throw new SimpleIdentityDataNotFoundException("User[" + userId + "] cannot be found"); }
            string nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            auths.First().Nonce = nonce;
            await _dbContext.SaveChangesAsync();
            return nonce;
        }

        public async Task<Token> GenerateTokenBySignature(int userId, int clientId, string sign)
        {
            var auths = await _dbContext.Users
                .Where(b => b.Id == userId &&
                    b.IsActive)
                .Select(b => new { User = b, Authentication = b.Authentication }).ToListAsync();
            var clients = await _dbContext.Clients
                .Where(b => b.Id == clientId)
                .Select(b => b).ToListAsync();
            if (!auths.Any()) { throw new SimpleIdentityDataNotFoundException("User[" + userId + "] cannot be found"); }
            if (!clients.Any()) { throw new SimpleIdentityDataNotFoundException("Client[" + clientId + "] cannot be found"); }

            UserAuthentication auth = auths.First().Authentication;
            User user = auths.First().User;
            Client client = clients.First();
            if (auth.Nonce == null) { throw new SimpleIdentityDataException("Please generate nonce before accquiring a token"); }
            string computedSignature = Hash(client.Secret + auth.Nonce + auth.Secret);

            auth.Nonce = null;
            await _dbContext.SaveChangesAsync();

            if (computedSignature != sign) { throw new SimpleIdentityDataException("Invalid signature[" + sign + "]"); }

            return await GenerateTokenInternal(user, client, true);
        }

        public async Task<Token> GenerateTokenByPassword(int userId, string userSecret, int clientId, string clientSecret)
        {
            var users = await _dbContext.Users
                .Where(b => b.Id == userId &&
                    b.IsActive &&
                    b.Authentication.Secret == userSecret)
                .Select(b => b).ToListAsync();
            var clients = await _dbContext.Clients
                .Where(b => b.Id == clientId &&
                    b.Secret == clientSecret)
                .Select(b => b).ToListAsync();
            if (!users.Any()) { throw new SimpleIdentityDataNotFoundException("Invalid user credentials"); }
            if (!clients.Any()) { throw new SimpleIdentityDataNotFoundException("Invalid client credentials"); }

            return await GenerateTokenInternal(users.First(), clients.First(), false);
        }

        private async Task<Token> GenerateTokenInternal(User user, Client client, bool requireSignature)
        {
            Token token = new Token
            {
                AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                TokenTimeout = DateTime.Now.AddSeconds(60),
                User = user,
                Client = client
            };
            _dbContext.Tokens.Add(token);
            await _dbContext.SaveChangesAsync();

            return token;
        }

        public async Task<Token> RefreshToken(string accessToken, string refreshToken)
        {
            var tokens = await _dbContext.Tokens
                .Where(b => b.AccessToken == accessToken &&
                    b.RefreshToken == refreshToken &&
                    b.User.IsActive)
                .Select(b => b).ToListAsync();
            if (!tokens.Any()) { throw new SimpleIdentityDataNotFoundException("Token cannot be found"); }

            Token token = tokens.First();
            token.AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            token.RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            token.TokenTimeout = DateTime.Now.AddSeconds(60);

            await _dbContext.SaveChangesAsync();

            return token;
        }

        private static string Hash(string data)
        {
            HMACSHA256 hash = new HMACSHA256(UTF8Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash.Hash);
        }
    }
}
