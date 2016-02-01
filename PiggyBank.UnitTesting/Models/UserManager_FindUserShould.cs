using System;
using Xunit;

using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Models
{
    public class UserManager_FindUserShould
    {
        [Fact]
        public void ThrowDataNotFoundException_WhenUserNotFound()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                userManager.FindUser(1);
            }
            catch (PiggyBankDataNotFoundException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public void ReturnUser_WhenUserIdMatch()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            User found = userManager.FindUser(user.Id);

            Assert.True(found.Id == user.Id);
            Assert.True(found.Name == user.Name);
        }
    }
}
