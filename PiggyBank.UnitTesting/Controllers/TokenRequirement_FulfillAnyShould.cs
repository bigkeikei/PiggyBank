using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using PiggyBank.Models;
using PiggyBank.Controllers;
using PiggyBank.UnitTesting.Mocks;

namespace PiggyBank.UnitTesting.Controllers
{
    public class TokenRequirement_FulfillAnyShould
    {
        [Fact]
        public async void ReturnTrue_WhenTokenIsSelfOwned()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);
            PiggyBankRepository repo = new PiggyBankRepository(mockDbContext);

            List<TokenRequirement> reqs = new List<TokenRequirement>();
            reqs.Add(new TokenRequirement { ResourceId = 1, ResourceType = Token.TokenResourceType.User, Scopes = Token.TokenScopes.Readable });
            bool fulfill = await TokenRequirement.FulfillAny(repo, "Bearer 1", reqs);

            Assert.True(fulfill);
        }

        [Fact]
        public async void ReturnTrue_WhenTokenIsReadable()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);
            PiggyBankRepository repo = new PiggyBankRepository(mockDbContext);

            List<TokenRequirement> reqs = new List<TokenRequirement>();
            reqs.Add(new TokenRequirement { ResourceId = 2, ResourceType = Token.TokenResourceType.User, Scopes = Token.TokenScopes.Readable });
            bool fulfill = await TokenRequirement.FulfillAny(repo, "Bearer happycatcanreadskinypig", reqs);

            Assert.True(fulfill);
        }

        [Fact]
        public async void ReturnFalse_WhenAuthorizationIsNotProvided()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);
            PiggyBankRepository repo = new PiggyBankRepository(mockDbContext);

            List<TokenRequirement> reqs = new List<TokenRequirement>();
            reqs.Add(new TokenRequirement { ResourceId = 1, ResourceType = Token.TokenResourceType.User, Scopes = Token.TokenScopes.Readable });
            bool fulfill = await TokenRequirement.FulfillAny(repo, null, reqs);

            Assert.True(!fulfill);
        }

        [Fact]
        public async void ReturnFalse_WhenTokenIsNotMatching()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);
            PiggyBankRepository repo = new PiggyBankRepository(mockDbContext);

            List<TokenRequirement> reqs = new List<TokenRequirement>();
            reqs.Add(new TokenRequirement { ResourceId = 1, ResourceType = Token.TokenResourceType.User, Scopes = Token.TokenScopes.Readable });
            bool fulfill = await TokenRequirement.FulfillAny(repo, "Bearer Some Funny Token", reqs);

            Assert.True(!fulfill);
        }

        [Fact]
        public async void ReturnFalse_WhenTokenIsExpired()
        {
            var mockDbContext = new MockPiggyBankDbContext(MockData.Seed());
            UserManager userManager = new UserManager(mockDbContext);
            PiggyBankRepository repo = new PiggyBankRepository(mockDbContext);

            List<TokenRequirement> reqs = new List<TokenRequirement>();
            reqs.Add(new TokenRequirement { ResourceId = 1, ResourceType = Token.TokenResourceType.User, Scopes = Token.TokenScopes.Readable });
            bool fulfill = await TokenRequirement.FulfillAny(repo, "skinypigcanreadhappycatbefore", reqs);

            Assert.True(!fulfill);
        }
        
    }
}
