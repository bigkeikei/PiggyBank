using Moq;
using System;
using Xunit;

using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;


namespace PiggyBank.UnitTesting.Models
{
    public class UserManager_UpdateUserShould
    {
        [Fact]
        public void ThrowException_WhenUserNotProvided()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                userManager.UpdateUser(null);
            }
            catch (PiggyBankDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public void ThrowException_WhenEditingUserName()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
                UserManager userManager = new UserManager(mockDbContext);

                var rand = new Random(Guid.NewGuid().GetHashCode());
                User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
                User modifiedUser = new User();
                modifiedUser.Id = user.Id;
                modifiedUser.IsActive = user.IsActive;
                modifiedUser.Email = user.Email;
                modifiedUser.Name = user.Name + " with some changes";

                userManager.UpdateUser(modifiedUser);
            }
            catch (PiggyBankDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public void ThrowException_WhenMandatoryElementIsMissing()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
                UserManager userManager = new UserManager(mockDbContext);

                var rand = new Random(Guid.NewGuid().GetHashCode());
                User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
                User modifiedUser = new User();
                modifiedUser.Id = user.Id;
                modifiedUser.IsActive = user.IsActive;
                modifiedUser.Name = user.Name;

                userManager.UpdateUser(modifiedUser);
            }
            catch (PiggyBankDataException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public void ReturnUser_WhenSuccessful()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            User modifiedUser = new User();
            modifiedUser.Id = user.Id;
            modifiedUser.IsActive = user.IsActive;
            modifiedUser.Email = "someone@newemail.com";
            modifiedUser.Name = user.Name;

            user = userManager.UpdateUser(modifiedUser);

            Assert.Equal(1, mockDbContext.SaveCount);
            Assert.True(user.Id == modifiedUser.Id);
            Assert.True(user.IsActive == modifiedUser.IsActive);
            Assert.True(user.Email == modifiedUser.Email);
            Assert.True(user.Name == modifiedUser.Name);
        }
    }
}