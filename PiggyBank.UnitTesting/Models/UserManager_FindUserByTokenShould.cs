using System;
using System.Linq;
using Xunit;

using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Models
{
    public class UserManager_FindUserByTokenShould
    {
        [Fact]
        public async void ThrowException_WhenUserNotFound()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.FindUserByToken("Some token");
            }
            catch (PiggyBankDataNotFoundException e) { ex = e; }

            Assert.True(ex != null);

        }

        [Fact]
        public async void ReturnUser_WhenUserTokenMatch()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            var token = (from b in user.Tokens.AsQueryable()
                           where b.ResourceType == Token.TokenResourceType.User &&
                           b.ResourceId == user.Id &&
                           b.Scopes == Token.TokenScopes.Full &&
                           b.User.Id == user.Id
                           select b).ToList();
            Assert.True(token.Any());
            User found = await userManager.FindUserByToken(token.First().AccessToken);

            Assert.True(found.Id == user.Id);
            Assert.True(found.Name == user.Name);
        }
    }
}
