using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;

namespace PiggyBank.Controllers
{
    public class TokenRequirement
    {
        IPiggyBankRepository _repo;
        string _authorization;
        Token.TokenResourceType _resourceType;
        int _resourceId;
        Token.TokenScope[] _scopes;

        public TokenRequirement(IPiggyBankRepository repo, string authorization, Token.TokenResourceType resourceType, int resourceId, Token.TokenScope[] scopes)
        {
            _repo = repo;
            _authorization = authorization;
            _resourceType = resourceType;
            _resourceId = resourceId;
            _scopes = scopes;
        }

        public async Task<Token> Fulfill()
        {
            try
            {
                if (_authorization == null) { throw new PiggyBankUserException("Authorization not provided"); }
                if (!_authorization.StartsWith("Bearer ")) { throw new PiggyBankUserException("Invalid authorization"); }
                return await _repo.UserManager.CheckAccessToken(_authorization.Substring(7), _resourceType, _resourceId, _scopes);
            }
            catch (PiggyBankAuthenticationTimeoutException e) { throw new PiggyBankUserException(e.Message); }
            catch (PiggyBankDataException e) { throw new PiggyBankUserException(e.Message); }
        }

        public static async Task<Token> Fulfill(IPiggyBankRepository repo, string authorization, Token.TokenResourceType resourceType, int resourceId, Token.TokenScope[] scopes)
        {
            TokenRequirement requirement = new TokenRequirement(repo, authorization, resourceType, resourceId, scopes);
            return await requirement.Fulfill();
        }
    }
}
