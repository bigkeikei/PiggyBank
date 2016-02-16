using System.Collections.Generic;

using PiggyBank.Models;
using System;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockData
    {
        public MockData()
        {
            Users = new List<User>();
            Books = new List<Book>();
            Accounts = new List<Account>();
            Transactions = new List<Transaction>();
            Tokens = new List<Token>();
        }

        public List<User> Users { get; set; }
        public List<Book> Books { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Token> Tokens { get; set; }

        public static MockData Seed()
        {
            MockData data = new MockData();
            int userId = 0;
            int tokenId = 0;
            data.Users.Add(new User { Id = ++userId, Name = "Happy Cat", Email = "cat@happy.com", IsActive = true });
            data.Users.Add(new User { Id = ++userId, Name = "Skiny Pig", Email = "pig@skiny.com", IsActive = true });
            data.Users.Add(new User { Id = ++userId, Name = "Silly Dog", Email = "dog@silly.com", IsActive = true });
            foreach (User user in data.Users)
            {
                user.Authentication = new UserAuthentication { Id = user.Id, User = user, Challenge = "haha", ChallengeTimeout = DateTime.Now, Secret = "secret" };
                user.Tokens = new List<Token>();
                user.Tokens.Add(new Token { Id = ++tokenId, User = user, AccessToken = tokenId.ToString(), ResourceType = Token.TokenResourceType.User, ResourceId = user.Id, Scope = Token.TokenScope.Full, TokenTimeout = DateTime.Now.AddSeconds(60) });
                data.Tokens.AddRange(user.Tokens);
            }

            return data;
        }
    }
}
