using Moq;
using System;
using Xunit;

using SimpleIdentity.Models;
using SimpleIdentity.UnitTesting.Mocks;


namespace SimpleIdentity.UnitTesting.Models
{
    public class UserManager_UpdateUserShould
    {
        [Fact]
        public async void ThrowException_WhenUserNotProvided()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.UpdateUser(null);
            }
            catch (SimpleIdentityDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ThrowException_WhenEditingUserName()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext(MockData.Seed());
                UserManager userManager = new UserManager(mockDbContext);

                var rand = new Random(Guid.NewGuid().GetHashCode());
                User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
                User modifiedUser = new User();
                modifiedUser.Id = user.Id;
                modifiedUser.IsActive = user.IsActive;
                modifiedUser.Email = user.Email;
                modifiedUser.Name = user.Name + " with some changes";

                await userManager.UpdateUser(modifiedUser);
            }
            catch (SimpleIdentityDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ThrowException_WhenMandatoryElementIsMissing()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockSimpleIdentityDbContext(MockData.Seed());
                UserManager userManager = new UserManager(mockDbContext);

                var rand = new Random(Guid.NewGuid().GetHashCode());
                User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
                User modifiedUser = new User();
                modifiedUser.Id = user.Id;
                modifiedUser.IsActive = user.IsActive;
                modifiedUser.Name = user.Name;

                await userManager.UpdateUser(modifiedUser);
            }
            catch (SimpleIdentityDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ReturnUser_WhenSuccessful()
        {
            var mockDbContext = new MockSimpleIdentityDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            User modifiedUser = new User();
            modifiedUser.Id = user.Id;
            modifiedUser.IsActive = user.IsActive;
            modifiedUser.Email = "someone@newemail.com";
            modifiedUser.Name = user.Name;

            user = await userManager.UpdateUser(modifiedUser);

            Assert.Equal(1, mockDbContext.SaveCount);
            Assert.True(user.Id == modifiedUser.Id);
            Assert.True(user.IsActive == modifiedUser.IsActive);
            Assert.True(user.Email == modifiedUser.Email);
            Assert.True(user.Name == modifiedUser.Name);
        }
    }
}