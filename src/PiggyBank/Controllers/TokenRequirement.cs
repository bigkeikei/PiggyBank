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
        int _userId;
        string _authorization;

        public TokenRequirement(IPiggyBankRepository repo, int userId, string authorization)
        {
            _repo = repo;
            _userId = userId;
            _authorization = authorization;
        }

        public User Fulfill()
        {
            try
            {
                if (_authorization == null) { throw new PiggyBankUserException("Authorization not provided"); }
                if (!_authorization.StartsWith("Bearer ") { throw new PiggyBankUserException("Invalid authorization"); }
                User user = _repo.UserManager.FindUser(_userId);
                return _repo.UserManager.CheckAccessToken(_userId, _authorization.Substring(7));
            }
            catch (PiggyBankAuthenticationTimeoutException e) { throw new PiggyBankUserException(e.Message); }
            catch (PiggyBankDataException e) { throw new PiggyBankUserException(e.Message); }
        }

        public static User Fulfill(IPiggyBankRepository repo, int userId, string authorization)
        {
            TokenRequirement requirement = new TokenRequirement(repo, userId, authorization);
            return requirement.Fulfill();
        }
    }
}
