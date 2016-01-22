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
            //if (_authorization == null) throw new PiggyBankUserException("Authorization not provided");
            User user = _repo.UserManager.FindUser(_userId);
            //if ( _authorization != "Bearer " + user.Authentication.AccessToken) throw new PiggyBankUserException("Incorrect authorization [" + _authorizaion + "]");
            return user;
        }

        public static User Fulfill(IPiggyBankRepository repo, int userId, string authorization)
        {
            TokenRequirement requirement = new TokenRequirement(repo, userId, authorization);
            return requirement.Fulfill();
        }
    }
}
