using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;

namespace PiggyBank.Controllers
{
    public class TokenRequirement
    {
        public Token.TokenResourceType ResourceType { get; set; }
        public int ResourceId { get; set; }
        public Token.TokenScopes Scopes { get; set; }

        public async Task<bool> Fulfill(IPiggyBankRepository repo, string authorization)
        {
            try
            {
                if (authorization == null) { return false; }
                if (!authorization.StartsWith("Bearer ")) { return false; }
                return await repo.UserManager.CheckAccessToken(authorization.Substring(7), ResourceType, ResourceId, Scopes);
            }
            catch (PiggyBankAuthenticationTimeoutException e) { throw new PiggyBankUserException(e.Message); }
            catch (PiggyBankDataException e) { throw new PiggyBankUserException(e.Message); }
        }

        public static async Task<bool> FulfillAny(IPiggyBankRepository repo, string authorization, IEnumerable<TokenRequirement> requirements)
        {
            foreach(TokenRequirement requirement in requirements)
            {
                if (await requirement.Fulfill(repo, authorization)) { return true; }
            }
            return false;
        }
    }
}
