using System;
using Xunit;

using PiggyBank.Models;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Models
{
    public class UserManager_GenerateChallengeShould
    {
        [Fact]
        public async void ThrowDataNotFoundException_WhenUserNotFound()
        {
            Exception ex = null;
            try
            {
                var mockDbContext = new MockPiggyBankDbContext();
                UserManager userManager = new UserManager(mockDbContext);
                await userManager.GenerateChallenge(1);
            }
            catch (PiggyBankDataNotFoundException e) { ex = e; }

            Assert.True(ex != null);
        }

        [Fact]
        public async void ReturnAuthentication_WhenSuccessful()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);

            var rand = new Random(Guid.NewGuid().GetHashCode());
            User user = mockDbContext.Data.Users[rand.Next(0, mockDbContext.Data.Users.Count - 1)];
            UserAuthentication oldAuth = new UserAuthentication();
            if (user.Authentication != null)
            {
                oldAuth.Challenge = user.Authentication.Challenge;
                oldAuth.ChallengeTimeout = user.Authentication.ChallengeTimeout;
            }
            UserAuthentication newAuth = await userManager.GenerateChallenge(user.Id);

            Assert.NotEqual(oldAuth.Challenge, newAuth.Challenge);
            Assert.NotEqual(oldAuth.ChallengeTimeout, newAuth.ChallengeTimeout);
            Assert.True(newAuth.Challenge != null);
            Assert.True(newAuth.ChallengeTimeout != null);
        }
    }
}
