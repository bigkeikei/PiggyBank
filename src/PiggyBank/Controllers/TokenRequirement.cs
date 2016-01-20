using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;
using Microsoft.AspNet.Mvc;

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
            // bypass for testing
            return _repo.FindUser(_userId);

            if (_authorization == null) return null;
            User user = _repo.FindUser(_userId);
            if ( user == null || _authorization != "Bearer " + user.Authentication.AccessToken)
            {
                return null;
            }
            return user;
        }

        public static User Fulfill(IPiggyBankRepository repo, int userId, string authorization)
        {
            TokenRequirement requirement = new TokenRequirement(repo, userId, authorization);
            return requirement.Fulfill();
        }
    }
}
