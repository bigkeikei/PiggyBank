using System;
using Xunit;

using SimpleIdentity.Models;
using SimpleIdentity.UnitTesting.Mocks;

namespace SimpleIdentity.UnitTesting.Models
{
    public class UserManager_FindUserShould
    {
        [Fact]
        public async void ThrowDataNotFoundException_WhenUserNotFound()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.FindUser(1);
            }
            catch (SimpleIdentityDataNotFoundException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ReturnUser_WhenUserIdMatch()
        {
            var mockDbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            User found = await userManager.FindUser(user.Id);

            Assert.True(found.Id == user.Id);
            Assert.True(found.Name == user.Name);
        }
    }
}
