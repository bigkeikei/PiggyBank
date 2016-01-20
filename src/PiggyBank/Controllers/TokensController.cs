using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class TokensController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("challenge/{username}")]
        public IActionResult GetChallenge(string userName)
        {
            User user = Repo.FindUserByName(userName);
            if (user == null)
            {
                return HttpNotFound(new { error = "User [" + userName + "] not found" });
            }
            UserAuthentication auth = Repo.GenerateChallenge(user.Id);
            return new ObjectResult(new { Challenge = auth.Challenge });
        }

        [HttpGet("token/{username}")]
        public IActionResult GetToken(string userName, [FromQuery] string signature)
        {
            User user = Repo.FindUserByName(userName);
            if (user == null)
            {
                return HttpNotFound(new { Error = "User [" + userName + "] not found" });
            }
            if (signature != user.Authentication.Signature)
            {
                return HttpBadRequest(new { Error = "Invalid signature [" + signature + "]" });
            }
            return new ObjectResult(Repo.GenerateToken(user.Id));
        }
    }
}
