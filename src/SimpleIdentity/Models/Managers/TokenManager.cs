﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SimpleIdentity.Models
{
    public class TokenManager : ITokenManager
    {
        private ISimpleIdentityDbContext _dbContext;

        public TokenManager(ISimpleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}