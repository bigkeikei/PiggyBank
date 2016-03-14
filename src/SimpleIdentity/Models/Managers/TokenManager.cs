using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SimpleIdentity.Models
{
    public class TokenManager : ITokenManager
    {
        private ISimpleIdentityDbContext _dbContext;

        public TokenManager(ISimpleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> ComputeSignature(string accessToken, string method, string url, Stream body = null, Dictionary<string, string> parameters = null)
        {
            var tokens = await _dbContext.Tokens
                .Where(b => b.AccessToken == accessToken &&
                    b.TokenTimeout > DateTime.Now)
                .Include(b => b.Client)
                .Select(b => b)
                .ToListAsync();
            if (!tokens.Any()) { throw new SimpleIdentityDataNotFoundException("Token[" + accessToken + "] cannot be found"); }
            Token token = tokens.First();
            if (!token.RequireSignature) { return null; }

            Dictionary<string, string> dataDict = (parameters == null ? new Dictionary<string, string>() : new Dictionary<string, string>(parameters));
            dataDict.Add("accessToken", accessToken);
            dataDict.Add("clientId", token.Client.Id.ToString());
            dataDict.Add("clientSecret", token.Client.Secret);
            if (body != null)
            {
                StreamReader r = new StreamReader(body);
                dataDict.Add("httpBody", r.ReadToEnd());
            }
            dataDict.Add("httpMethod", method);
            dataDict.Add("httpUrl", url);
            return Hash(dataDict.OrderBy(b => b.Key).Select(b => b.Value));
        }

        public async Task<Token> GenerateTokenBySignature(int userId, int clientId, string nonce, string signature)
        {
            var users = await _dbContext.Users
                .Where(b => b.Id == userId &&
                    b.IsActive)
                .Include(b => b.Authentication)
                .ToListAsync();
            var nonces = await _dbContext.UserNonces
                .Where(b => b.User.Id == userId &&
                    b.Nonce == nonce &&
                    b.Timeout >= DateTime.Now &&
                    b.IsValid)
                .ToListAsync();
            var clients = await _dbContext.Clients
                .Where(b => b.Id == clientId)
                .Select(b => b).ToListAsync();
            if (!users.Any()) { throw new SimpleIdentityDataNotFoundException("User[" + userId + "] cannot be found"); }
            if (!clients.Any()) { throw new SimpleIdentityDataNotFoundException("Client[" + clientId + "] cannot be found"); }
            if (!nonces.Any()) { throw new SimpleIdentityDataNotFoundException("Nonce[" + nonce + "] cannot be found or is invalid"); }

            User user = users.First();
            Client client = clients.First();
            string computedSignature = Hash(new string[] { client.Secret, nonce, user.Authentication.Secret });

            nonces.First().IsValid = false;
            await _dbContext.SaveChangesAsync();

            if (computedSignature != signature) { throw new SimpleIdentityDataException("Invalid signature[" + signature + "]"); }

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
                TokenTimeout = DateTime.Now.AddSeconds(900),
                User = user,
                Client = client,
                RequireSignature = requireSignature
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

        private static string Hash(IEnumerable<string> data)
        {
            string dataString = "";
            foreach(string str in data) { dataString += str; }
            var hash = SHA256.Create();
            return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(dataString)));
        }
    }
}
