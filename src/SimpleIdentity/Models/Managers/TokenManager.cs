using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SimpleIdentity.Models
{
    public class TokenManager
    {
        private ISimpleIdentityDbContext _dbContext;

        public TokenManager(ISimpleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Token> GenerateToken(int userId, string userSecret, int clientId, string clientSecret)
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

            Token token = new Token
            {
                AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                TokenTimeout = DateTime.Now.AddSeconds(60),
                User = users.First(),
                Client = clients.First()
            };

            _dbContext.Tokens.Add(token);
            await _dbContext.SaveChangesAsync();

            return token;
        }
    }
}
